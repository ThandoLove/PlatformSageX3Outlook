using Microsoft.AspNetCore.Authorization;

namespace OperationalWorkspaceAPI.Policies;

public static class PermissionPolicies
{
    // ======================================================
    // ENTERPRISE STRATEGIC CLAIM ASSIGNMENT KEYS
    // ======================================================
    public const string AdminOnly = "AdminOnly";
    public const string ManagerOrAbove = "ManagerOrAbove";
    public const string CanCreate = "CanCreate";
    public const string CanApprove = "CanApprove";
    public const string CanView = "CanView";
    public const string CanInvoice = "CanInvoice";
    public const string CanInventory = "CanInventory";

    public static void Register(AuthorizationOptions options)
    {
        // ======================================================
        // ROLE FALLBACK POLICIES
        // ======================================================
        options.AddPolicy(AdminOnly, policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy(ManagerOrAbove, policy =>
            policy.RequireRole("Admin", "Manager"));

        // ======================================================
        // ERP MODULE POLICIES
        // ======================================================
        options.AddPolicy(CanCreate, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("Admin") ||
                context.User.IsInRole("Manager") ||
                context.User.IsInRole("User")));

        options.AddPolicy(CanApprove, policy =>
            policy.RequireRole("Admin", "Manager"));

        options.AddPolicy(CanView, policy =>
            policy.RequireRole("Admin", "Manager", "User", "ERPUser"));

        options.AddPolicy(CanInvoice, policy =>
            policy.RequireRole("Admin", "ERPUser"));

        options.AddPolicy(CanInventory, policy =>
            policy.RequireRole("Admin", "ERPUser"));
    }
}
