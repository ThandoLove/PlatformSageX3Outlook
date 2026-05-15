
using OperationalWorkspaceAPI.AuditAPI;

namespace OperationalWorkspaceAPI.Middleware;

public class AuditEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public AuditEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context,
        IAuditLogger logger)
    {
        var audit = new AuditEvent
        {
            UserId = context.User?.FindFirst("UserId")?.Value,
            Email = context.User?.Identity?.Name,

            Company = context.User?.FindFirst("Company")?.Value,
            Dataset = context.User?.FindFirst("Dataset")?.Value,

            Source = context.Request.Path,
            HttpMethod = context.Request.Method,

            IpAddress = context.Connection.RemoteIpAddress?.ToString(),

            Action = AuditEventType.ApiRequest,

            TimestampUtc = DateTime.UtcNow,

            Success = true
        };

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            audit.Success = false;
            audit.ErrorMessage = ex.Message;

            await logger.LogAsync(audit);

            throw;
        }

        await logger.LogAsync(audit);
    }
}