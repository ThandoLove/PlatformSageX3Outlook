
using OperationalWorkspaceAPI.Middleware;

namespace OperationalWorkspaceAPI.ApiExtensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseWorkspaceMiddleware(this IApplicationBuilder app)
    {
        // 1. Performance first to time the whole pipeline
        app.UseMiddleware<PerformanceTrackingMiddleware>();

        // 2. Correlation ID for tracking
        app.UseMiddleware<RequestCorrelationMiddleware>();

        // 3. RBAC to identify the user
        app.UseMiddleware<RbacMiddleware>();

        // 4. Audit Logging last (so it knows the User and the Time)
        app.UseMiddleware<AuditLoggingMiddleware>();

        return app;
    }
}
