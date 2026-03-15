using System.Diagnostics;

namespace OperationalWorkspaceAPI.Middleware;

public sealed class PerformanceTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceTrackingMiddleware> _logger;

    public PerformanceTrackingMiddleware(RequestDelegate next, ILogger<PerformanceTrackingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > 500) // Alert on slow requests
            {
                _logger.LogWarning("PERF ALERT: {Method} {Path} took {Elapsed}ms",
                    context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
            }
        }
    }
}