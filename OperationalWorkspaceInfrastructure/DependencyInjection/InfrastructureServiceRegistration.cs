using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspaceApplication.Interfaces.BackgroundJobsApp; // Added this using directive
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Interfaces;
using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.Http;
using OperationalWorkspaceInfrastructure.Persistence;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
using OperationalWorkspaceInfrastructure.servicesInfra.BackgroundWorkers;
using System;

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
                // Core Application DbContext Mapping
                services.AddDbContext<IntegrationDbContext>(options =>
                    options.UseSqlServer(
                        conn,
                        b => b.MigrationsAssembly(typeof(IntegrationDbContext).Assembly.FullName)
                    ));

                // 🚀 STABILIZED: Explicitly register your Security Cryptographic Keys DB using the same SQL connection
                services.AddDbContext<DataProtectionDbContext>(options =>
                    options.UseSqlServer(
                        conn,
                        b => b.MigrationsAssembly(typeof(DataProtectionDbContext).Assembly.FullName)
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

                // 🚀 STABILIZED: Fallback database setup for security keys on connection exceptions
                services.AddDbContext<DataProtectionDbContext>(options =>
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

            // 🚀 STABILIZED: Development fallback in-memory registration for isolated testing tracking
            services.AddDbContext<DataProtectionDbContext>(options =>
            {
                options.UseInMemoryDatabase("SecurityInMemory");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });
        }

        services.AddInfrastructureLayer();

        return services;
    }

    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        // ======================================================
        // BACKGROUND WORKER REGISTRATION (Priority 2)
        // ======================================================
        services.AddScoped<ISageSyncJobs, SageSyncJobs>();

        // 🚀 NEW: Register the Background Queue as a Singleton across the application lifetime
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueueService>();

        // 🚀 NEW: Register the continuous background engine hosted process thread
        services.AddHostedService<QueuedHostedService>();

        // Repositories (100% PRESERVED RELEVANT MAPPINGS FROM YOUR EXISTING CODE)
        services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
       
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
      
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();

        // Caching & ERP Authentication
        services.AddScoped<IDistributedTokenCacheService, DistributedTokenCacheService>();
        services.AddScoped<ISageAuthService, SageAuthService>();

        // SageX3 Plumbing
        services.AddScoped<ISageX3Client, SageX3Client>();
        services.AddScoped<ISageX3AttachmentService, SageX3AttachmentService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpClient<OperationalWorkspaceApplication.Services.BusinessPartnerService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<ISageHttpClient, SageHttpClient>();

        return services;
    }
}
