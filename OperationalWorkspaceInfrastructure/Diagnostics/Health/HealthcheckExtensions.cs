using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OperationalWorkspaceInfrastructure.Diagnostics.Health
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddEnterpriseHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddTypeActivatedCheck<SageX3HealthCheck>("SageX3-ERP-Link", failureStatus: HealthStatus.Degraded, tags: new[] { "ready" });

            return services;
        }

        public static void MapCustomHealthEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"status\":\"Alive\"}");
                }
            });

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
        }
    }
}
