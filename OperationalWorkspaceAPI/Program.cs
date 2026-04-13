using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.Middleware;
using OperationalWorkspaceAPI.Policies;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.DependencyInjection;
using OperationalWorkspaceInfrastructure.Services;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
using OperationalWorkspaceAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORE INFRASTRUCTURE ---
builder.Services.AddApiLayer();
builder.Services.AddWorkspaceSwagger();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers();

// --- 2. JWT AUTHENTICATION (PRODUCTION READY) ---
// This replaces the simple API Key logic with industry-standard security
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

// --- 3. SAGE X3 & INFRASTRUCTURE SERVICES ---
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

// Map SageSecurityOptions and Attachment Paths
builder.Services.Configure<OperationalWorkspaceInfrastructure.Configuration.SageSecurityOptions>(
    builder.Configuration.GetSection("SageSecurityOptions"));

var attachmentPath = builder.Configuration["SageX3:AttachmentPath"] ?? "C:\\Temp\\SageAttachments";
builder.Services.AddSingleton(attachmentPath);

builder.Services.AddInfrastructureServices(builder.Configuration);

// --- 4. CORS POLICY ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("OutlookAddInPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7123", "http://localhost:5065")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// --- 5. DEPENDENCY INJECTION (MOCKS & REPOS) ---
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

    builder.Services.AddScoped<IUserContextService, UserContextService>();
    builder.Services.AddScoped<IIntegrationService, IntegrationService>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    // Inside Program.cs on the Server
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();

}

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

var app = builder.Build();

// --- 6. MIDDLEWARE PIPELINE (ORDER IS CRITICAL) ---

// A. Infrastructure/Diagnostics first
app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<PerformanceTrackingMiddleware>(); // Track time for the whole pipeline

if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();
}

app.UseHttpsRedirection();
app.UseCors("OutlookAddInPolicy");

// B. Security Layer (Must come before RBAC and Audit)
app.UseAuthentication(); // Decodes the JWT
app.UseAuthorization();  // Checks [Authorize] attributes

// C. Custom Business Middleware (Now has access to User claims from JWT)
app.UseMiddleware<RbacMiddleware>();        // Checks roles
app.UseMiddleware<AuditLoggingMiddleware>(); // Logs the identified User
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();
