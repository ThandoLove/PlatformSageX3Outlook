using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace OperationalWorkspaceAPI.ApiExtensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddWorkspaceSwagger(this IServiceCollection services)
    {
        // Register default Swagger generator. Keep configuration minimal to avoid type/version mismatches.
        services.AddSwaggerGen();
        return services;
    }

    public static IApplicationBuilder UseWorkspaceSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Operational Workspace API v1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}
