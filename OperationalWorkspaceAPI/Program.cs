using FluentValidation;
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
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.Configuration;
using OperationalWorkspaceInfrastructure.DependencyInjection;
using OperationalWorkspaceInfrastructure.Diagnostics;
using OperationalWorkspaceInfrastructure.Diagnostics.Health;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;
using OperationalWorkspaceInfrastructure.Persistence;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
using OperationalWorkspaceInfrastructure.Resilience;
using OperationalWorkspaceInfrastructure.SecurityInfrastructure;
using OperationalWorkspaceInfrastructure.Services;
using OperationalWorkspaceInfrastructure.services;
using OperationalWorkspaceShared.Validators;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1. CORE INFRASTRUCTURE
// ======================================================
builder.Services.AddApiLayer();
builder.Services.AddWorkspaceSwagger();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// ======================================================
// 2. PRODUCTION COMPRESSION & METRICS TELEMETRY
// ======================================================
builder.Services.AddProductionCompression();
builder.Services.AddEnterpriseTelemetry(builder.Configuration, "OperationalWorkspace-API");
builder.Services.AddEnterpriseHealthChecks(builder.Configuration);

// ======================================================
// 3. ENTERPRISE RATE LIMITING CONFIGURATION
// ======================================================
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

// ======================================================
// 4. VALIDATION
// ======================================================
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();

// ======================================================
// 5. SECURITY & TENANT CONTEXT
// ======================================================
builder.Services.AddScoped<ISecurityContext, SecurityContext>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();

// ======================================================
// 6. AUTH PROVIDER
// ======================================================
builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();

// ======================================================
// 7. JWT CONFIGURATION & VALIDATION LAYERS (HARDENED SECURE)
// ======================================================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PlatformSageX3OutlookBackend";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PlatformSageX3OutlookAddin";

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("CRITICAL CONFIGURATION ERROR: Cryptographic JWT Master Signing Key is missing from the environment configuration layers.");
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

// ======================================================
// 8. AUTHORIZATION
// ======================================================
builder.Services.AddAuthorization(options =>
{
    PermissionPolicies.Register(options);
});

// ======================================================
// 9. SAGE CONFIGURATION
// ======================================================
var sageConfig = builder.Configuration.GetSection("SageX3");
var baseUrl = sageConfig["RestBaseUrl"] ?? "https://syracuse-server.com";

if (string.IsNullOrWhiteSpace(baseUrl))
{
    throw new InvalidOperationException("SageX3 RestBaseUrl is not configured.");
}

// ======================================================
// 10. SAGE ATTACHMENT OPTIONS
// ======================================================
builder.Services.Configure<SageX3AttachmentOptions>(builder.Configuration.GetSection("SageX3"));

// ======================================================
// 11. POLLY RESILIENCE
// ======================================================
var sagePolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(30));

var sageCircuitBreaker = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// ======================================================
// 12. SAGE CLIENTS & ADAPTER ENVIRONMENT ISOLATION
// ======================================================
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

// ======================================================
// 13. SAGE REST SERVICE
// ======================================================
builder.Services
    .AddHttpClient<ISageRestService, SageRestService>(client =>
    {
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(sagePolicy)
    .AddPolicyHandler(sageCircuitBreaker);

// ======================================================
// 14. OPTIONS
// ======================================================
builder.Services.Configure<SageSecurityOptions>(builder.Configuration.GetSection("SageSecurityOptions"));

// ======================================================
// 15. INFRASTRUCTURE SERVICE LAYER REGISTRATION
// ======================================================
InfrastructureServiceRegistration.AddInfrastructureServices(builder.Services, builder.Configuration);

// ======================================================
// 16. AUDIT LAYER STORAGE
// ======================================================
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<AuditContext>();

// ======================================================
// 17. CORS POLICY HARDENING
// ======================================================
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

// ======================================================
// 18. APPLICATION SERVICES ENVIRONMENT BINDINGS
// ======================================================
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<MockUnifiedService>();

    builder.Services.AddScoped<IActivityService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IEmailService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IKnowledgeService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IInvoiceService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<ISalesService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IBusinessPartnerService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<IInventoryService>(sp => sp.GetRequiredService<MockUnifiedService>());
    builder.Services.AddScoped<ITaskService>(sp => sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IAuditLogService, MockAuditService>();
    builder.Services.AddScoped<ISystemHealthService, MockSystemHealthService>();
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
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddScoped<IAuditLogService, AuditLogService>();
    builder.Services.AddScoped<IAttachmentService, AttachmentService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
}

// ======================================================
// 19. REPOSITORIES
// ======================================================
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// ======================================================
// 20. BUSINESS SERVICES
// ======================================================
builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<JwtTokenService>();

// ======================================================
// 21. BUILD EXECUTABLE APPLICATION CONTEXT
// ======================================================
var app = builder.Build();

// ======================================================
// HEALTH CHECKS (SMOKE TESTS)
// ======================================================
app.MapGet("/health", () => Results.Json(new { status = "ok" }));
app.MapGet("/health/live", () => Results.Json(new { status = "Alive" }));
app.MapGet("/health/ready", () => Results.Json(new { status = "Ready" }));

// ======================================================
// 22. NETWORK REVERSE PROXY FORWARDED HEADERS
// ======================================================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// ======================================================
// 23. CORE PERFORMANCE RESPONSE COMPRESSION
// ======================================================
app.UseResponseCompression();

// ======================================================
// 24. GLOBAL EXCEPTION HANDLING & SECURITY MATRIX
// ======================================================
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<EnterpriseSecurityHeadersMiddleware>();

app.UseHttpsRedirection();

// ======================================================
// 25. STATIC ASSET MANAGEMENT
// ======================================================
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// ======================================================
// 26. DOCUMENTATION PLATFORM CONFIGURATIONS
// ======================================================
if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();
}

// ======================================================
// 27. SECURITY ROUTING PIPELINE SEQUENCE
// ======================================================
app.UseRouting();
app.UseRateLimiter();
app.UseCors("OutlookAddInPolicy");
app.UseAuthentication();
app.UseAuthorization(); // Confirms explicit security alignments

// ======================================================
// 28. STRATEGIC CONTEXT AUDITING MIDDLEWARE BLOCK
// ======================================================
app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenantIsolationMiddleware>();
app.UseMiddleware<AuditEnrichmentMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<PerformanceTrackingMiddleware>();

// ======================================================
// 29. SYSTEM ENDPOINT CONTROLLER ASSIGNMENTS
// ======================================================
app.MapControllers();
app.MapCustomHealthEndpoints();
app.MapFallbackToFile("index.html");

// ======================================================
// 30. METRICS ENDPOINT
// ======================================================
app.MapGet("/metrics", async context =>
{
    context.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";

    await context.Response.WriteAsync(
        "# HELP application_up Process status metric.\n" +
        "# TYPE application_up gauge\n" +
        "application_up 1\n");
});

// ======================================================
// 31. RUN PROCESS
// ======================================================
app.Run();

