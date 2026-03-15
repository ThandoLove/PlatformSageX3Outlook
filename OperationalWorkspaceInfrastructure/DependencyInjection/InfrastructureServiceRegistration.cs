using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.Http;
using OperationalWorkspaceInfrastructure.Persistence;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;

namespace OperationalWorkspaceInfrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    // FIX: This provides the method Program.cs is calling
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Register the DbContext (Fixes the SQL Server connection)
        services.AddDbContext<IntegrationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(IntegrationDbContext).Assembly.FullName)));

        // 2. Call the layer registration below
        services.AddInfrastructureLayer();

        return services;
    }

    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();

        // Caching
        services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();

        // ERP Authentication
        services.AddScoped<ISageAuthService, SageAuthService>();

        // SageX3
        services.AddScoped<ISageX3Client, SageX3Client>();
        services.AddScoped<ISageX3AttachmentService, SageX3AttachmentService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpClient<ISageHttpClient, SageHttpClient>();

        return services;
    }
}
