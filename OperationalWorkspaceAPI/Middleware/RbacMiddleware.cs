using System.Security.Claims;

namespace OperationalWorkspaceAPI.Middleware;

public sealed class RbacMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RbacMiddleware> _logger;

    public RbacMiddleware(RequestDelegate next, ILogger<RbacMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // ==============================
        // PUBLIC ROUTES ONLY
        // ==============================
        if (path.StartsWith("/health") ||
            path.StartsWith("/api/v1/Auth"))
        {
            await _next(context);
            return;
        }

        // ==============================
        // AUTH CHECK ONLY
        // ==============================
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authentication Required.");
            return;
        }

        // ==============================
        // LOG ACCESS (NO BLOCKING)
        // ==============================
        var user = context.User.Identity?.Name ?? "Unknown";

        _logger.LogInformation(
            "API ACCESS: User {User} accessed {Path}",
            user, path);

        await _next(context);
    }
}