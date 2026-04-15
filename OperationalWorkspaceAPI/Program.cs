using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.Middleware;
using OperationalWorkspaceAPI.Policies;
using OperationalWorkspaceAPI.Services;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.DependencyInjection;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
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
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();

// --- 2. JWT AUTHENTICATION ---
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
var sageConfig = builder.Configuration.GetSection("SageX3");

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ISageRestService, MockSageRestService>();
}
else
{
    builder.Services.AddHttpClient<ISageRestService, SageRestService>(client => {
        client.BaseAddress = new Uri(sageConfig["RestBaseUrl"] ?? "https://localhost");
    });
}

builder.Services.Configure<OperationalWorkspaceInfrastructure.Configuration.SageSecurityOptions>(
    builder.Configuration.GetSection("SageSecurityOptions"));

var attachmentPath = builder.Configuration["SageX3:AttachmentPath"] ?? "C:\\Temp\\SageAttachments";
builder.Services.AddSingleton(attachmentPath);
builder.Services.AddInfrastructureServices(builder.Configuration);

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
    builder.Services.AddScoped<IIntegrationService, IntegrationService>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
}

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

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

app.UseMiddleware<RbacMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Fallback to the Blazor index.html for any non-API routes
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
