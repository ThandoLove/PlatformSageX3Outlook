
using System.Security.Claims;
using System.Text;

namespace OperationalWorkspaceAPI.Middleware;

public sealed class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only audit Mutating requests (POST, PUT, DELETE)
        if (context.Request.Method == HttpMethods.Get || context.Request.Method == HttpMethods.Options)
        {
            await _next(context);
            return;
        }

        var user = context.User.Identity?.Name ?? "Anonymous";
        var path = context.Request.Path;
        var method = context.Request.Method;
        var traceId = context.TraceIdentifier;

        // Shield: Wrap in try/catch so logging failure doesn't kill the transaction
        try
        {
            _logger.LogInformation("AUDIT START: User {User} performing {Method} on {Path} | Trace: {TraceId}",
                user, method, path, traceId);

            await _next(context);

            _logger.LogInformation("AUDIT SUCCESS: {Method} {Path} completed with Status {Status}",
                method, path, context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "AUDIT FAILURE: Critical error during operation {TraceId}", traceId);
            throw; // Re-throw to be caught by ApiExceptionFilter
        }
    }
}
