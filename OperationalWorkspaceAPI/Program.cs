using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceAPI.ApiExtensions;
using OperationalWorkspaceAPI.Middleware;
using OperationalWorkspaceAPI.Policies;

using OperationalWorkspaceInfrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

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
}
else
{
    // Real Infrastructure (Database, SQL Server, etc.)
    // This call lives in InfrastructureServiceRegistration.cs
    builder.Services.AddInfrastructureServices(builder.Configuration);
}

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
// DO NOT ADD ANY CODE BELOW THIS LINE. 
// ANY EXTRA CLASSES MUST LIVE IN THEIR OWN FILES.
