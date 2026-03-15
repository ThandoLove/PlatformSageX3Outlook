namespace OperationalWorkspaceAPI.Middleware;

public sealed class RequestCorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeader = "X-Correlation-ID";

    public RequestCorrelationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if upstream (Gateway/Proxy) already sent a Trace ID
        if (!context.Request.Headers.TryGetValue(CorrelationHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Apply to current Context for the ApiController and ExceptionFilter to use
        context.TraceIdentifier = correlationId;

        // Apply to Response so the client/frontend can report it to support
        context.Response.Headers[CorrelationHeader] = correlationId;

        await _next(context);
    }
}
