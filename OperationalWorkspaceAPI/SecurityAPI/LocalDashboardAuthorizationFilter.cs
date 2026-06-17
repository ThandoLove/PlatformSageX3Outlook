
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace OperationalWorkspaceAPI.SecurityAPI;

public class LocalDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _env;

    public LocalDashboardAuthorizationFilter(IWebHostEnvironment env)
    {
        _env = env;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (_env.IsDevelopment()
            && (httpContext.Request.Host.Host == "localhost"
                || httpContext.Request.Host.Host == "127.0.0.1"))
        {
            return true;
        }

        return httpContext.User.Identity?.IsAuthenticated == true
               && (httpContext.User.IsInRole("Admin") || httpContext.User.IsInRole("Manager"));
    }
}
