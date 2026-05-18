using System.Security.Claims;

namespace OperationalWorkspaceAPI.Middleware;

public class TenantIsolationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantIsolationMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public TenantIsolationMiddleware(
        RequestDelegate next,
        ILogger<TenantIsolationMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        // ==============================
        // READ CLAIMS
        // ==============================
        var company = context.User?.FindFirst("Company")?.Value;
        var dataset = context.User?.FindFirst("Dataset")?.Value;

        // ==============================
        // DEV MODE SAFE FALLBACK
        // ==============================
        if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(dataset))
        {
            if (_env.IsDevelopment())
            {
                // ✔ DEV ONLY: do NOT block requests
                company = company ?? "DEV-COMPANY";
                dataset = dataset ?? "DEV-DATASET";

                _logger.LogWarning(
                    "DEV MODE: Missing tenant claims. Using fallback values. Path: {Path}",
                    context.Request.Path);
            }
            else
            {
                // ✔ PRODUCTION: enforce strict security
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Missing tenant context (Company/Dataset)");
                return;
            }
        }

        // ==============================
        // SET CONTEXT (USED BY APP)
        // ==============================
        context.Items["Company"] = company;
        context.Items["Dataset"] = dataset;

        await _next(context);
    }
}