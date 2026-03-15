using Microsoft.AspNetCore.Authorization;
using System.Runtime.Intrinsics.X86;


// Policies/PermissionPolicies.cs

namespace OperationalWorkspaceAPI.Policies;

public static class PermissionPolicies
{
    public static void Register(AuthorizationOptions options)
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole(RolePolicies.Admin));
        options.AddPolicy("ERPWrite", policy => policy.RequireRole(RolePolicies.Admin, RolePolicies.Manager));
    }
}


