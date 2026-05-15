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

// --- 1. CORE INFRASTRUCTURE ---
builder.Services.AddApiLayer();
builder.Services.AddWorkspaceSwagger();
builder.Services.AddDistributedMemoryCache();

// FLUENTVALIDATION: Register validators and enable auto-validation
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<ISecurityContext, SecurityContext>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();

// --- 2. JWT AUTHENTICATION ---

/* 
   FUTURE AZURE AD SETUP:
   1. Install NuGet: Microsoft.Identity.Web
   2. Replace the code below with:
   builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
*/
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "fallback_secret_key_for_dev_only_32_chars"))
    };
});

builder.Services.AddAuthorization(options => PermissionPolicies.Register(options));

// --- 3. INFRASTRUCTURE & SAGE X3 ---
// --- 3. INFRASTRUCTURE & SAGE X3 ---
var sageConfig = builder.Configuration.GetSection("SageX3");

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ISageX3Client, MockSageX3Client>();
}
else
{
    builder.Services.AddHttpClient<ISageX3Client, SageX3Client>(client => {
        client.BaseAddress = new Uri(sageConfig["RestBaseUrl"] ?? "https://localhost");
    });
}

builder.Services.AddHttpClient<
    ISageX3IdentityService,
    SageX3IdentityService>(client =>
    {
        client.BaseAddress = new Uri(
            sageConfig["RestBaseUrl"] ?? "https://localhost");
    });


builder.Services.Configure<OperationalWorkspaceInfrastructure.Configuration.SageSecurityOptions>(
    builder.Configuration.GetSection("SageSecurityOptions"));

var attachmentPath = builder.Configuration["SageX3:AttachmentPath"] ?? "C:\\Temp\\SageAttachments";
builder.Services.AddSingleton(attachmentPath);

// 🔥 FIX: Break the ambiguity by calling both static extension methods explicitly
InfrastructureServiceRegistration.AddInfrastructureServices(builder.Services, builder.Configuration);



// --- 4. CORS POLICY (Corrected Origins) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("OutlookAddInPolicy", policy =>
    {
        // 7173 matches your UI's HTTPS port from launchsettings
        policy.WithOrigins("https://localhost:7173", "http://localhost:5065")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// --- 5. DEPENDENCY INJECTION ---
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

    // ✅ ADD IT HERE
    builder.Services.AddScoped<ISystemHealthService, MockSystemHealthService>();

    builder.Services.AddScoped<IIntegrationService, IntegrationService>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IAuditLogger, AuditLogger>();


    builder.Services.AddScoped<UnifiedService>(provider =>
        new UnifiedService(
            activityRepo: provider.GetService<IActivityRepository>()!,
            sageClient: provider.GetRequiredService<ISageX3Client>(),
            dbContext: provider.GetRequiredService<IntegrationDbContext>(),
            logger: provider.GetRequiredService<ILogger<UnifiedService>>()
        ));
}
else
{
    // ✅ REAL HEALTH SERVICE
    builder.Services.AddScoped<ISystemHealthService, SystemHealthService>();
}

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

// --- 6. MIDDLEWARE PIPELINE (Order is Critical) ---
app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<PerformanceTrackingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();
    app.UseWebAssemblyDebugging(); // Enable debugging for Blazor WASM
}

app.UseHttpsRedirection();

// 🔥 CRITICAL: These allow the API to host the Blazor WebAssembly files
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("OutlookAddInPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<TenantIsolationMiddleware>();

app.UseMiddleware<AuditEnrichmentMiddleware>();

app.UseMiddleware<RbacMiddleware>();

app.UseMiddleware<AuditLoggingMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();

// Fallback to the Blazor index.html for any non-API routes
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
