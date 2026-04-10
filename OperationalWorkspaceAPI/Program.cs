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

// --- SAGE X3 CONFIGURATION ---
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

// 1. SERVICES CONFIGURATION
builder.Services.AddApiLayer();
builder.Services.AddWorkspaceSwagger();
builder.Services.AddDistributedMemoryCache();

// FIX 1: Map the SageSecurityOptions required by SageAuthService
builder.Services.Configure<OperationalWorkspaceInfrastructure.Configuration.SageSecurityOptions>(
    builder.Configuration.GetSection("SageSecurityOptions"));

// FIX 2: Explicitly provide the string required by SageX3AttachmentService constructor
// This pulls from SageX3:AttachmentPath in your appsettings.json
var attachmentPath = builder.Configuration["SageX3:AttachmentPath"] ?? "C:\\Temp\\SageAttachments";
builder.Services.AddSingleton(attachmentPath);

// REGISTER INFRASTRUCTURE
// This call now has access to the Options and the Path string registered above
builder.Services.AddInfrastructureServices(builder.Configuration);

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("OutlookAddInPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7173", "http://localhost:5065")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

if (builder.Environment.IsDevelopment())
{
    // Mock Services for local development
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
}

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddAuthorization(options => PermissionPolicies.Register(options));

var app = builder.Build();

// 2. MIDDLEWARE PIPELINE
app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseWorkspaceSwagger();
}

app.UseHttpsRedirection();
app.UseCors("OutlookAddInPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Advanced Custom Middleware
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<PerformanceTrackingMiddleware>();
app.UseMiddleware<RbacMiddleware>();

app.MapControllers();

app.Run();
