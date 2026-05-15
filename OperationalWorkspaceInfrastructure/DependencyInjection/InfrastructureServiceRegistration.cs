using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspaceApplication.IServices;
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
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(conn))
        {
            try
            {
                services.AddDbContext<IntegrationDbContext>(options =>
                    options.UseSqlServer(
                        conn,
                        b => b.MigrationsAssembly(typeof(IntegrationDbContext).Assembly.FullName)
                    ));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Warning: UseSqlServer failed, falling back to non-SQL DbContext. {ex.Message}"
                );

                services.AddDbContext<IntegrationDbContext>(options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                });
            }
        }
        else
        {
            Console.WriteLine(
                "Info: No DefaultConnection provided. IntegrationDbContext configured to use InMemory DB for development."
            );

            services.AddDbContext<IntegrationDbContext>(options =>
            {
                options.UseInMemoryDatabase("DevInMemory");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });
        }

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