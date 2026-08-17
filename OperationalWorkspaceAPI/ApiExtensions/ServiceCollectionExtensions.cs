using OperationalWorkspaceAPI.Filters;
using OperationalWorkspaceAPI.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;


namespace OperationalWorkspaceAPI.ApiExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ApiExceptionFilter>();
            options.Filters.Add<ValidationFilter>(); // The data shield
        });

        // Production Shield: API Versioning (Prevents breaking clients)
        // Configure reader and add the API Explorer so versions are described to Swagger
        services.AddApiVersioning(config => {
            config.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
            // Optimize for performance by using query string versioning (e.g. ?api-version=1.0)
            config.ApiVersionReader = new QueryStringApiVersionReader();
        });

        // Add versioned API explorer so Swagger can describe API versions
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
