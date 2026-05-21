
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OperationalWorkspaceInfrastructure.SecurityInfrastructure
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddProductionCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var productionOrigins = configuration.GetSection("Security:AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("StrictOutlookAddInPolicy", builder =>
                {
                    if (productionOrigins != null && productionOrigins.Length > 0)
                    {
                        builder.WithOrigins(productionOrigins);
                    }
                    else
                    {
                        builder.AllowAnyOrigin();
                    }

                    builder.AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
