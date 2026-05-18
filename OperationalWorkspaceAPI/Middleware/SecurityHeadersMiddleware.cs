
namespace OperationalWorkspaceAPI.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context)
    {
        context.Response.Headers["X-Frame-Options"]
            = "DENY";

        context.Response.Headers["X-Content-Type-Options"]
            = "nosniff";

        context.Response.Headers["Referrer-Policy"]
            = "strict-origin-when-cross-origin";

        context.Response.Headers["X-XSS-Protection"]
            = "1; mode=block";

        context.Response.Headers["Permissions-Policy"]
            = "camera=(), microphone=(), geolocation=()";

        context.Response.Headers["Content-Security-Policy"]
            =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' https: data:; " +
            "connect-src 'self' https: wss:; " +
            "frame-ancestors 'none';";

        await _next(context);
    }
}