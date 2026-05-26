
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace OperationalWorkspaceAPI.SecurityAPI;

public class LocalDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Always allow localhost debugging environments
        if (httpContext.Request.Host.Host == "localhost" ||
            httpContext.Request.Host.Host == "127.0.0.1")
        {
            return true;
        }

        // Restrict production panel access to authenticated Admins
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Administrator");
    }
}
