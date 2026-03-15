
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
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var roles = context.User.FindAll(System.Security.Claims.ClaimTypes.Role);
            _logger.LogInformation("User {User} accessing with roles: {Roles}",
                context.User.Identity.Name, string.Join(",", roles));
        }
        await _next(context);
    }
}
