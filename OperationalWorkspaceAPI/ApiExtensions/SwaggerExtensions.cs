using Microsoft.AspNetCore.Builder; // Required for IApplicationBuilder
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;// Use .Models for OpenApiInfo/Scheme
using System.Collections.Generic;

namespace OperationalWorkspaceAPI.ApiExtensions;

public static class SwaggerExtensions
{
    // This is for builder.Services
    public static IServiceCollection AddWorkspaceSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Operational Workspace API",
                Version = "v1"
            });

            // 1. Define the Security Scheme
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token."
            });

            // 2. Apply Requirement Globally using the new Delegate Pattern
            c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    new List<string>()
                }
            });
        });

        return services;
    }

    // FIX: ADD THIS METHOD SO THE ERROR IN PROGRAM.CS GOES AWAY
    public static IApplicationBuilder UseWorkspaceSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Operational Workspace API v1");
            c.RoutePrefix = "swagger"; // This makes it available at /swagger
        });
        return app;
    }
}
