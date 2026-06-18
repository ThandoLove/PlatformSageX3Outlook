using Microsoft.Extensions.DependencyInjection; // Add this line
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceApplication.Interfaces.IServices;


namespace OperationalWorkspaceApplication.Dependency_Injection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Add Attachment Service
        services.AddScoped<IAttachmentService, AttachmentService>();

        // Core ERP Services (Removed duplicates)
        services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
       
       
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}
