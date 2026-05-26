using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OperationalWorkspaceAPI.Middleware;

public class EnterpriseSecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public EnterpriseSecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Enforce modern security headers using fast .NET 8 Append operations
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("X-XSS-Protection", "0");
        context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");

        // CONTENT SECURITY POLICY (CSP) FOR OUTLOOK TASKPANES
        // Explicitly whitelists the exact secure transmission subdomains used by Microsoft 365 
        // to host Outlook Add-in canvases while preventing unauthorized clickjacking scripts.
        string csp = "default-src 'self'; " +
                     "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                     "style-src 'self' 'unsafe-inline'; " +
                     "img-src 'self' data: https:; " +
                     "font-src 'self' https: data:; " +
                     "connect-src 'self' https: wss:; " +
                     "frame-ancestors 'self' " +
                     "https://outlook.office.com " +
                     "https://outlook.office365.com " +
                     "https://appsforoffice.microsoft.com " +
                     "https://office.com " +
                     "https://office365.com;";

        context.Response.Headers.Append("Content-Security-Policy", csp);

        await _next(context);
    }
}
