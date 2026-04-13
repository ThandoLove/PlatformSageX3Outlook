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
        // 1. Skip check for non-protected paths (like Health Checks)
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // 2. Identification check
        if (context.User.Identity?.IsAuthenticated != true)
        {
            _logger.LogWarning("ACCESS DENIED: Unauthenticated request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authentication Required.");
            return;
        }

        // 3. Role Enforcement logic (Example: Admin required for DELETE/POST)
        var roles = context.User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(r => r.Value);
        var method = context.Request.Method;

        if ((method == "DELETE" || method == "POST") && !roles.Contains("Admin"))
        {
            _logger.LogWarning("FORBIDDEN: User {User} attempted {Method} without Admin role.",
                context.User.Identity.Name, method);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access Denied: Admin Role Required.");
            return;
        }

        await _next(context);
    }
}
