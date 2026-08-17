using Microsoft.AspNetCore.Builder; // Required for IApplicationBuilder
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi; // Use for OpenApiInfo/Scheme
using System.Collections.Generic;

namespace OperationalWorkspaceAPI.ApiExtensions;

public static class SwaggerExtensions
{
    // Register Swagger generation and add a Swagger document per discovered API version
    public static IServiceCollection AddWorkspaceSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Default/fallback doc
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Operational Workspace API",
                Version = "v1"
            });

            // Define the security scheme for Bearer JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token."
            });

            // Apply requirement globally
            c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    new List<string>()
                }
            });

            // If the versioned API explorer is available at startup, register docs for each discovered API version.
            using var sp = services.BuildServiceProvider();
            var provider = sp.GetService<IApiVersionDescriptionProvider>();
            if (provider != null)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = $"Operational Workspace API {description.GroupName}",
                        Version = description.ApiVersion.ToString(),
                        Description = "Operational Workspace API"
                    });
                }
            }
        });

        return services;
    }

    // Configure the Swagger UI and expose one endpoint per API version
    public static IApplicationBuilder UseWorkspaceSwagger(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            if (provider != null)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Operational Workspace API {description.GroupName}");
                }
            }
            else
            {
                // Fallback single endpoint
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Operational Workspace API v1");
            }

            c.RoutePrefix = "swagger"; // This makes it available at /swagger
        });

        return app;
    }
}
