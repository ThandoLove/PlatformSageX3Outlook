using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.AuditAPI;
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
using OperationalWorkspaceInfrastructure.SecurityInfrastructure;
using OperationalWorkspaceInfrastructure.services;
using OperationalWorkspaceInfrastructure.Services;
using OperationalWorkspaceShared.Validators;
using System.Text;

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
// 2. VALIDATION
// ======================================================

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddScoped<
    IValidator<LoginRequestDto>,
    LoginRequestValidator>();


// ======================================================
// 3. SECURITY CONTEXT SERVICES
// ======================================================

builder.Services.AddScoped<
    ISecurityContext,
    SecurityContext>();

builder.Services.AddScoped<
    ITenantContext,
    TenantContext>();

builder.Services.AddScoped<
    IUserContextService,
    UserContextService>();

builder.Services.AddScoped<
    IDistributedTokenCacheService,
    DistributedTokenCacheService>();


// ======================================================
// 4. JWT AUTHENTICATION
// ======================================================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key not configured.");
}

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Key must be at least 32 characters long.");
}

/*
 FUTURE AZURE AD SETUP:
 Install:
 Microsoft.Identity.Web

 Replace JWT config with:
 builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
*/

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });


// ======================================================
// 5. AUTHORIZATION
// ======================================================

builder.Services.AddAuthorization(options =>
{
    PermissionPolicies.Register(options);
});


// ======================================================
// 6. SAGE X3 CONFIGURATION
// ======================================================

var sageConfig =
    builder.Configuration.GetSection("SageX3");


// ======================================================
// 7. SAGE X3 CLIENTS
// ======================================================

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<
        ISageX3Client,
        MockSageX3Client>();
}
else
{
    builder.Services.AddHttpClient<
        ISageX3Client,
        SageX3Client>(client =>
        {
            client.BaseAddress =
                new Uri(
                    sageConfig["RestBaseUrl"]
                    ?? "https://localhost");
        });
}


// ======================================================
// SAGE REST SERVICE
// ======================================================

builder.Services.AddHttpClient<
    ISageRestService,
    SageRestService>(client =>
    {
        client.BaseAddress =
            new Uri(
                sageConfig["RestBaseUrl"]
                ?? "https://localhost");
    });

// ======================================================
// 8. SAGE IDENTITY SERVICE
// ======================================================

builder.Services.AddHttpClient<
    ISageX3IdentityService,
    SageX3IdentityService>(client =>
    {
        client.BaseAddress =
            new Uri(
                sageConfig["RestBaseUrl"]
                ?? "https://localhost");
    });


// ======================================================
// 9. OPTIONS CONFIGURATION
// ======================================================

builder.Services.Configure<
    OperationalWorkspaceInfrastructure.Configuration
        .SageSecurityOptions>(
            builder.Configuration.GetSection(
                "SageSecurityOptions"));


// ======================================================
// 10. ATTACHMENT STORAGE
// ======================================================

var attachmentPath =
    builder.Configuration["SageX3:AttachmentPath"]
    ?? "C:\\Temp\\SageAttachments";

builder.Services.AddSingleton(attachmentPath);


// ======================================================
// 11. INFRASTRUCTURE SERVICES
// ======================================================

InfrastructureServiceRegistration
    .AddInfrastructureServices(
        builder.Services,
        builder.Configuration);


// ======================================================
// 12. CORS
// ======================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "OutlookAddInPolicy",
        policy =>
        {
            policy
                .WithOrigins(
                    "https://localhost:7173",
                    "http://localhost:5065")

                .AllowAnyHeader()

                .AllowAnyMethod()

                .AllowCredentials();
        });
});


// ======================================================
// 13. APPLICATION SERVICES
// ======================================================

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<MockUnifiedService>();


    builder.Services.AddScoped<IActivityService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IEmailService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IKnowledgeService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IInvoiceService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<ISalesService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IBusinessPartnerService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<IInventoryService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());

    builder.Services.AddScoped<ITaskService>(sp =>
        sp.GetRequiredService<MockUnifiedService>());


    builder.Services.AddScoped<
        IAuditLogService,
        MockAuditService>();


    builder.Services.AddScoped<
        ISystemHealthService,
        MockSystemHealthService>();


    builder.Services.AddScoped<
        IIntegrationService,
        IntegrationService>();


    builder.Services.AddScoped<
        IAuditLogRepository,
        AuditLogRepository>();


    builder.Services.AddScoped<
        IAccountRepository,
        AccountRepository>();


    builder.Services.AddScoped<
        IAuditLogger,
        AuditLogger>();


    builder.Services.AddScoped<UnifiedService>(provider =>
        new UnifiedService(
            activityRepo:
                provider.GetService<IActivityRepository>()!,

            sageClient:
                provider.GetRequiredService<ISageX3Client>(),

            dbContext:
                provider.GetRequiredService<IntegrationDbContext>(),

            logger:
                provider.GetRequiredService<
                    ILogger<UnifiedService>>()
        ));
}
else
{
    builder.Services.AddScoped<
        ISystemHealthService,
        SystemHealthService>();
}


// ======================================================
// 14. REPOSITORIES
// ======================================================

builder.Services.AddScoped<
    ITicketRepository,
    TicketRepository>();


// ======================================================
// 15. TOKEN SERVICE
// ======================================================

builder.Services.AddScoped<JwtTokenService>();


// ======================================================
// 16. BUILD APPLICATION
// ======================================================

var app = builder.Build();


// ======================================================
// 17. MIDDLEWARE PIPELINE
// ======================================================

app.UseMiddleware<RequestCorrelationMiddleware>();

app.UseMiddleware<PerformanceTrackingMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();

    app.UseWebAssemblyDebugging();
}


app.UseHttpsRedirection();


// ======================================================
// HOST BLAZOR WASM FILES
// ======================================================

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseRouting();


// ======================================================
// CORS
// ======================================================

app.UseCors("OutlookAddInPolicy");


// ======================================================
// AUTHENTICATION / AUTHORIZATION
// ======================================================

app.UseAuthentication();

app.UseAuthorization();


// ======================================================
// CUSTOM MIDDLEWARE
// ======================================================

app.UseMiddleware<TenantIsolationMiddleware>();

app.UseMiddleware<AuditEnrichmentMiddleware>();

app.UseMiddleware<RbacMiddleware>();

app.UseMiddleware<AuditLoggingMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();


// ======================================================
// ROUTING
// ======================================================

app.MapControllers();

app.MapFallbackToFile("index.html");


// ======================================================
// RUN APPLICATION
// ======================================================

app.Run();