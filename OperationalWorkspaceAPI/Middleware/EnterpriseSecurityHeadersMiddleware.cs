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
        // Do not set X-Frame-Options: DENY — Outlook task panes embed this app in an iframe.
        // Framing is restricted via Content-Security-Policy frame-ancestors instead.
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("X-XSS-Protection", "0");
        context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

        if (context.Request.IsHttps)
        {
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");
        }

        string csp = "default-src 'self'; " +
                     "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://appsforoffice.microsoft.com; " +
                     "style-src 'self' 'unsafe-inline'; " +
                     "img-src 'self' data: https:; " +
                     "font-src 'self' https: data:; " +
                     "connect-src 'self' https: wss:; " +
                     "frame-ancestors 'self' " +
                     "https://outlook.office.com " +
                     "https://outlook.office365.com " +
                     "https://outlook.live.com " +
                     "https://appsforoffice.microsoft.com " +
                     "https://office.com " +
                     "https://office365.com;";

        context.Response.Headers.Append("Content-Security-Policy", csp);

        await _next(context);
    }
}
