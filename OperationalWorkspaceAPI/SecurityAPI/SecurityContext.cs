using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace OperationalWorkspaceAPI.SecurityAPI;

public class SecurityContext : ISecurityContext
{
    private readonly IHttpContextAccessor _http;

    public SecurityContext(IHttpContextAccessor http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    private ClaimsPrincipal? User => _http.HttpContext?.User;

    public string UserId => User?.FindFirst("UserId")?.Value ?? string.Empty;

    public string Email => User?.Identity?.Name ?? string.Empty;

    public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";

    public string Company => User?.FindFirst("Company")?.Value ?? string.Empty;

    public string Dataset => User?.FindFirst("Dataset")?.Value ?? string.Empty;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    // ======================================================
    // SAGE IDENTITY GOVERNANCE CLAIMS EXTRACTION
    // ======================================================
    // Inspects the active token claims context payload for explicit Sage verification mapping
    public bool IsSageUser =>
        bool.TryParse(User?.FindFirst("is_sage_user")?.Value, out var result) && result;

    // Dynamically captures the assigned target enterprise context folder string (e.g. "SEED" / "PROD")
    public string SageEnvironment =>
        User?.FindFirst("sage_env")?.Value ?? "SEED";
}
