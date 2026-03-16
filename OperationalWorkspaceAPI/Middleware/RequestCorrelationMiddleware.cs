using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Middleware;

public sealed class RequestCorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeader = "X-Correlation-ID";

    public RequestCorrelationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Safely extract or generate the ID
        if (!context.Request.Headers.TryGetValue(CorrelationHeader, out StringValues correlationId) ||
            StringValues.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // 2. Assign to TraceIdentifier (ensuring it's a string, not StringValues)
        // This fixes the null reference assignment warning
        context.TraceIdentifier = correlationId.ToString() ?? Guid.NewGuid().ToString();

        // 3. Use .Append() instead of [] to safely add the header to the response
        // This is the production standard for .NET 8/9/10
        context.Response.Headers.Append(CorrelationHeader, correlationId);

        await _next(context);
    }
}
