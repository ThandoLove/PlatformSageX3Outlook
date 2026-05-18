using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.AuditAPI;
using OperationalWorkspaceAPI.Authentication;
using OperationalWorkspaceAPI.Middleware;
using OperationalWorkspaceAPI.Policies;
using OperationalWorkspaceAPI.SecurityAPI;
using OperationalWorkspaceAPI.Services;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.DependencyInjection;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;
using OperationalWorkspaceInfrastructure.Persistence;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
using OperationalWorkspaceInfrastructure.Resilience;
using OperationalWorkspaceInfrastructure.SecurityInfrastructure;
using OperationalWorkspaceInfrastructure.services;
using OperationalWorkspaceInfrastructure.Services;
using OperationalWorkspaceShared.Validators;
using Polly;
using Polly.Extensions.Http;
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

// ======================================================
// 2. RATE LIMITING
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
});

// ======================================================
// 3. VALIDATION
// ======================================================
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();

// ======================================================
// 4. SECURITY CONTEXT
// ======================================================
builder.Services.AddScoped<ISecurityContext, SecurityContext>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();

// ======================================================
// 5. AUTH PROVIDER
// ======================================================
builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();

// ======================================================
// 6. JWT AUTH
// ======================================================
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT Key not configured.");

if (jwtKey.Length < 32)
    throw new InvalidOperationException("JWT Key must be at least 32 characters long.");

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
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ======================================================
// 7. AUTHORIZATION
// ======================================================
builder.Services.AddAuthorization(options =>
{
    PermissionPolicies.Register(options);
});

// ======================================================
// 8. SAGE CONFIGURATION
// ======================================================
var sageConfig = builder.Configuration.GetSection("SageX3");
var baseUrl = sageConfig["RestBaseUrl"];

if (string.IsNullOrWhiteSpace(baseUrl))
    throw new InvalidOperationException("SageX3 RestBaseUrl is not configured.");

// ======================================================
// 9. POLLY RESILIENCE
// ======================================================
var sagePolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));

var sageCircuitBreaker = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// ======================================================
// 10. SAGE CLIENTS
// ======================================================
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ISageX3Client, MockSageX3Client>();
    builder.Services.AddScoped<ISageX3IdentityService, MockSageX3IdentityService>();
}
else
{
    builder.Services
        .AddHttpClient<ISageX3IdentityService, SageX3IdentityService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(sagePolicy)
        .AddPolicyHandler(sageCircuitBreaker);
}

// ======================================================
// 11. SAGE REST SERVICE
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
// 12. OPTIONS
// ======================================================
builder.Services.Configure<OperationalWorkspaceInfrastructure.Configuration.SageSecurityOptions>(
    builder.Configuration.GetSection("SageSecurityOptions"));

// ======================================================
// 13. ATTACHMENT STORAGE
// ======================================================
var attachmentPath =
    builder.Configuration["SageX3:AttachmentPath"]
    ?? "C:\\SecureStorage\\SageAttachments";

builder.Services.AddSingleton(attachmentPath);

// ======================================================
// 14. INFRASTRUCTURE
// ======================================================
InfrastructureServiceRegistration.AddInfrastructureServices(
    builder.Services,
    builder.Configuration);

// ======================================================
// 15. AUDIT
// ======================================================
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<AuditContext>();

// ======================================================
// 16. CORS
// ======================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("OutlookAddInPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(
                    "https://outlook.office.com",
                    "https://outlook.office365.com",
                    "https://localhost:7173",
                    "http://localhost:5065",
                    "https://your-production-domain.com"
                )
                .WithMethods("GET", "POST", "PUT", "DELETE")
                .WithHeaders("Authorization", "Content-Type")
                .AllowCredentials();
        }
    });
});

// ======================================================
// 17. APPLICATION SERVICES
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

    builder.Services.AddScoped< IInventoryService, InventoryService>();

    builder.Services.AddScoped<ITaskService, TaskService>();

    builder.Services.AddScoped<IAuditLogService, AuditLogService>();

    builder.Services.AddScoped<IAttachmentService, AttachmentService>();

    builder.Services.AddScoped<IOrderService, OrderService>();
}

// ======================================================
// 18. REPOSITORIES
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
// 19. BUSINESS SERVICES
// ======================================================
builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<JwtTokenService>();

// ======================================================
// 20. BUILD APP
// ======================================================
var app = builder.Build();

// ======================================================
// 21. FORWARDED HEADERS
// ======================================================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

// ======================================================
// 22. GLOBAL EXCEPTION HANDLING
// ======================================================
app.UseMiddleware<GlobalExceptionMiddleware>();

// ======================================================
// 23. HTTPS + SECURITY HEADERS
// ======================================================
app.UseHttpsRedirection();
app.UseEnterpriseSecurityHeaders();
// ======================================================
// 24. STATIC FILES
// ======================================================
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// ======================================================
// SWAGGER (only in Development)
// ======================================================
if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();
}

// ======================================================
// 25. ROUTING
// ======================================================
app.UseRouting();

// ======================================================
// 26. RATE LIMITING
// ======================================================
app.UseRateLimiter();

// ======================================================
// 27. CORS
// ======================================================
app.UseCors("OutlookAddInPolicy");

// ======================================================
// 28. AUTH
// ======================================================
app.UseAuthentication();
app.UseAuthorization();

// ======================================================
// 29. SECURITY + AUDIT PIPELINE
// ======================================================
//app.UseMiddleware<TenantIsolationMiddleware>();//
app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<AuditEnrichmentMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<PerformanceTrackingMiddleware>();

// ======================================================
// 30. ENDPOINTS
// ======================================================
app.MapControllers();
app.MapFallbackToFile("index.html");

// ======================================================
// 31. RUN
// ======================================================
app.Run();