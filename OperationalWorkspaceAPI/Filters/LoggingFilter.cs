
using System.Diagnostics;

namespace OperationalWorkspaceAPI.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;

        try
        {
            await _next(context);
            sw.Stop();

            // Production Standard: Different log levels based on result
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning("Request {Method} {Path} failed with {Status} in {Ms}ms | Trace: {TraceId}",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds, traceId);
            }
            else
            {
                _logger.LogInformation("Request {Method} {Path} finished in {Ms}ms",
                    context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
            }
        }
        catch (Exception)
        {
            sw.Stop(); // Ensure timer stops even on crash
            throw;
        }
    }
}
