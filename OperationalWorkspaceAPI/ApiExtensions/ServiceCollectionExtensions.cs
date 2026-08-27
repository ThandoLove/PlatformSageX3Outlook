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
        // Register API explorer and Swagger generator (simplified - no API versioning)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
