
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.Middleware;
using OperationalWorkspaceAPI.Policies;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.DependencyInjection;
using OperationalWorkspaceInfrastructure.Services;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- SAGE X3 CONFIGURATION ---
var sageConfig = builder.Configuration.GetSection("SageX3");
builder.Services.AddHttpClient<ISageRestService, SageRestService>(client => {
    // This uses the "RestBaseUrl" you added to appsettings.json
    client.BaseAddress = new Uri(sageConfig["RestBaseUrl"] ?? "https://localhost");
});
// -----------------------------

// 1. SERVICES CONFIGURATION
builder.Services.AddApiLayer();
builder.Services.AddWorkspaceSwagger(); // Defined in SwaggerExtensions.cs

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

    //real implementations for these services, even in development, since they don't depend on a database


    builder.Services.AddScoped<IUserContextService, UserContextService>();
    builder.Services.AddScoped<IIntegrationService, IntegrationService>();
    
   


    // Also register the Audit Repo for testing
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
}
else
{
    // Real Infrastructure (Database, SQL Server, etc.)
    // This call lives in InfrastructureServiceRegistration.cs
    builder.Services.AddInfrastructureServices(builder.Configuration);
}

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddAuthorization(options => PermissionPolicies.Register(options));

var app = builder.Build();

// 2. MIDDLEWARE PIPELINE
app.UseMiddleware<RequestCorrelationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Defined in SwaggerExtensions.cs
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
