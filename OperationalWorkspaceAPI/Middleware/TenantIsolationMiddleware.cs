
namespace OperationalWorkspaceAPI.Middleware;

public class TenantIsolationMiddleware
{
    private readonly RequestDelegate _next;

    public TenantIsolationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var company = context.User?.FindFirst("Company")?.Value;
        var dataset = context.User?.FindFirst("Dataset")?.Value;

        if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(dataset))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Missing tenant context");
            return;
        }

        context.Items["Company"] = company;
        context.Items["Dataset"] = dataset;

        await _next(context);
    }
}