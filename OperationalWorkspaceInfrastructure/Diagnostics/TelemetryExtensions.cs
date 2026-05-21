using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OperationalWorkspaceInfrastructure.Diagnostics
{
    public static class TelemetryExtensions
    {
        public static IServiceCollection AddEnterpriseTelemetry(this IServiceCollection services, IConfiguration configuration, string serviceName)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                })
                .WithMetrics(metrics =>
                {
                    metrics.SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                });

            return services;
        }
    }
}
