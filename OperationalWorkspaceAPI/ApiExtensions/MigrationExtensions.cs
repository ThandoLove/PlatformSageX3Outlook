
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OperationalWorkspaceInfrastructure.Persistence;
using System;

namespace OperationalWorkspaceAPI.ApiExtensions
{
    public static class MigrationExtensions
    {
        /// <summary>
        /// Self-healing automated runtime bootstrapper checking and updating your multi-tenant Entity Framework database tables.
        /// </summary>
        public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<IntegrationDbContext>>();

            try
            {
                logger.LogInformation("Analyzing application data persistence layers for pending database structural modifications...");

                var integrationDbContext = services.GetRequiredService<IntegrationDbContext>();

                // Triggers structural evolution updates safely on initialization to coordinate orchestration scripts
                if (integrationDbContext.Database.IsRelational() && integrationDbContext.Database.GetPendingMigrations().Any())
                {
                    logger.LogWarning("Pending Entity Framework migrations found. Executing zero-downtime structural database upgrades...");
                    integrationDbContext.Database.Migrate();
                    logger.LogInformation("Database structural modifications successfully processed and synchronized.");
                }
                else
                {
                    logger.LogInformation("Database persistence layer schemas match current application baseline structures completely.");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "A fatal exception was encountered while trying to process automated database migrations.");
                throw;
            }
        }
    }
}
