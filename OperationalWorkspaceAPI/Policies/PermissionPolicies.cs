using Microsoft.AspNetCore.Authorization;

namespace OperationalWorkspaceAPI.Policies;

public static class PermissionPolicies
{
    public static void Register(AuthorizationOptions options)
    {
        // ==========================
        // ROLE FALLBACK POLICIES
        // ==========================
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ManagerOrAbove", policy =>
            policy.RequireRole("Admin", "Manager"));

        // ==========================
        // ERP MODULE POLICIES
        // ==========================
        options.AddPolicy("CanCreate", policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("Admin") ||
                context.User.IsInRole("Manager") ||
                context.User.IsInRole("User")));

        options.AddPolicy("CanApprove", policy =>
            policy.RequireRole("Admin", "Manager"));

        options.AddPolicy("CanView", policy =>
            policy.RequireRole("Admin", "Manager", "User", "ERPUser"));

        options.AddPolicy("CanInvoice", policy =>
            policy.RequireRole("Admin", "ERPUser"));

        options.AddPolicy("CanInventory", policy =>
            policy.RequireRole("Admin", "ERPUser"));
    }
}