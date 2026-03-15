
using OperationalWorkspaceAPI.Middleware;

namespace OperationalWorkspaceAPI.ApiExtensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseWorkspaceMiddleware(this IApplicationBuilder app)
    {
        // Production Order: Infrastructure -> Security -> Business Logic
        app.UseMiddleware<RequestCorrelationMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<AuditLoggingMiddleware>();

        return app;
    }
}
