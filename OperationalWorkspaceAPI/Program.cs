using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.AuditAPI;
using OperationalWorkspaceAPI.Authentication;
using OperationalWorkspaceAPI.Middleware;
using OperationalWorkspaceAPI.Performance;
using OperationalWorkspaceAPI.Policies;
using OperationalWorkspaceAPI.SecurityAPI;
using OperationalWorkspaceAPI.Services;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.Providers;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.Configuration;
using OperationalWorkspaceInfrastructure.DependencyInjection;
using OperationalWorkspaceInfrastructure.Diagnostics;
using OperationalWorkspaceInfrastructure.Diagnostics.Health;
using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.SageConfiguration;
using OperationalWorkspaceInfrastructure.Persistence;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
using OperationalWorkspaceInfrastructure.Resilience;
using OperationalWorkspaceInfrastructure.SecurityInfrastructure;
using OperationalWorkspaceInfrastructure.services;
using OperationalWorkspaceInfrastructure.servicesInfra;
using OperationalWorkspaceApplication.Validators;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApiLayer();
builder.Services.AddWorkspaceSwagger();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(o => o.DetailedErrors = true);
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
});

builder.Services.AddProductionCompression();
builder.Services.AddEnterpriseTelemetry(builder.Configuration, "OperationalWorkspace-API");
builder.Services.AddEnterpriseHealthChecks(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

options.AddFixedWindowLimiter("GlobalPolicy", opt =>
{
    opt.PermitLimit = 100;
    opt.Window = TimeSpan.FromMinutes(1);
    opt.QueueLimit = 10;
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
});

options.AddFixedWindowLimiter("LoginPolicy", opt =>
{
    opt.PermitLimit = 5;
    opt.Window = TimeSpan.FromMinutes(1);
    opt.QueueLimit = 2;
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
});

options.AddFixedWindowLimiter("SagePolicy", opt =>
{
    opt.PermitLimit = 30;
    opt.Window = TimeSpan.FromMinutes(1);
    opt.QueueLimit = 5;
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
});
    options.AddPolicy("FixedWindowLimitPolicy", httpContext =>
    {
        var partitionKey = httpContext.User.Identity?.Name
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "Anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 5,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<CustomerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmailInsightDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TicketValidator>();

builder.Services.AddScoped<ISecurityContext, SecurityContext>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();

builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PlatformSageX3OutlookBackend";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PlatformSageX3OutlookAddin";

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("CRITICAL CONFIGURATION ERROR: Cryptographic JWT Master Signing Key is missing.");
}

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT Key must be at least 32 characters long.");
}
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = builder.Environment.IsProduction(),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    PermissionPolicies.Register(options);
});

var sageConfig = builder.Configuration.GetSection("SageX3");
builder.Services.Configure<SageSettings>(sageConfig);
var baseUrl = sageConfig["RestBaseUrl"] ?? "https://syracuse-server.com";

if (string.IsNullOrWhiteSpace(baseUrl))
{
    throw new InvalidOperationException("SageX3 RestBaseUrl is not configured.");
}

builder.Services.Configure<SageX3AttachmentOptions>(builder.Configuration.GetSection("SageX3"));

var sagePolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(30));

var sageCircuitBreaker = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ISageX3Client, MockSageX3Client>();
}
else
{
    builder.Services
        .AddHttpClient<ISageX3Client, SageX3Client>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(sagePolicy)
        .AddPolicyHandler(sageCircuitBreaker);
}
builder.Services
    .AddHttpClient<ISageRestService, SageRestService>(client =>
    {
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(sagePolicy)
    .AddPolicyHandler(sageCircuitBreaker);

builder.Services.Configure<SageSecurityOptions>(builder.Configuration.GetSection("SageSecurityOptions"));

InfrastructureServiceRegistration.AddInfrastructureServices(builder.Services, builder.Configuration);

builder.Services.AddScoped<MockAttachmentProvider>();
builder.Services.AddScoped<SageAttachmentProvider>();
builder.Services.AddScoped<IAttachmentProvider, SmartAttachmentProvider>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<AuditContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OutlookAddInPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        else
        {
            policy
                .WithOrigins(
                    "https://office.com",
                    "https://office365.com",
                    "https://localhost:7173",
                    "http://localhost:5065",
                    "https://your-production-domain.com")
                .WithMethods("GET", "POST", "PUT", "DELETE")
                .WithHeaders("Authorization", "Content-Type")
                .AllowCredentials();
        }
    });
});
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<MockUnifiedService>();

    builder.Services.AddScoped<IActivityService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IEmailService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IKnowledgeService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IInvoiceService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<ISalesService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IBusinessPartnerService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<ITaskService>(sp => sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IAuditLogService, MockAuditService>();
    builder.Services.AddScoped<ISystemHealthService, MockSystemHealthService>();
    builder.Services.AddScoped<ISageX3Client, MockSageX3Client>();
    builder.Services.AddScoped<ISageRestService, MockSageRestService>();
    builder.Services.AddScoped<ISageAuthService, MockSageAuthService>();

    // 🚀 FIXED FOR DEVELOPMENT MODE: Registers the builder right into the active mock container [INDEX]
    builder.Services.AddScoped<EmailContextBuilder>();
}
else
{
    builder.Services.AddScoped<ISystemHealthService, SystemHealthService>();
    builder.Services.AddScoped<IActivityService, ActivityService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IKnowledgeService, KnowledgeService>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<ISalesService, SalesService>();
    builder.Services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    // 🚀 FIXED FOR PRODUCTION MODE: Registers the builder right into the true core container [INDEX]
    builder.Services.AddScoped<EmailContextBuilder>();
}

builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<JwtTokenService>();

// 🚀 FIXED: Registers the system clock dependency needed to activate your Attachment Service [INDEX]
builder.Services.AddScoped<IClock, SystemClock>();

// 🚀 FIXED: Registers the context builder dependency required by the Dashboard page UI [INDEX]
builder.Services.AddScoped<EmailContextBuilder>();

var app = builder.Build();

app.MapGet("/health", () => Results.Json(new { status = "ok" }));
app.MapGet("/health/live", () => Results.Json(new { status = "Alive" }));
app.MapGet("/health/ready", () => Results.Json(new { status = "Ready" }));

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseResponseCompression();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<EnterpriseSecurityHeadersMiddleware>();

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new LocalDashboardAuthorizationFilter(app.Environment) }
});

app.UseRouting();
app.UseRateLimiter();
app.UseCors("OutlookAddInPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenantIsolationMiddleware>();
app.UseMiddleware<AuditEnrichmentMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<PerformanceTrackingMiddleware>();

app.MapControllers();
app.MapCustomHealthEndpoints();

app.MapRazorComponents<OperationalWorkspaceUI.Components.App>()
    .AddInteractiveServerRenderMode();
app.MapFallbackToFile("index.html");

app.MapGet("/metrics", async context =>
{
    context.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";

    await context.Response.WriteAsync(
        "# HELP application_up Process status metric.\n" +
        "# TYPE application_up gauge\n" +
        "application_up 1\n");
});

app.Run();
