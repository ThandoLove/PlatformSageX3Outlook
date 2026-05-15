using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace OperationalWorkspaceAPI.SecurityAPI;

public class SecurityContext : ISecurityContext
{
    private readonly IHttpContextAccessor _http;

    public SecurityContext(IHttpContextAccessor http)
    {
        _http = http;
    }

    private ClaimsPrincipal? User => _http.HttpContext?.User;

    // FIX: Match the interface's 'string' return type and use ?? to safely handle nulls
    public string UserId => User?.FindFirst("UserId")?.Value ?? string.Empty;

    public string Email => User?.Identity?.Name ?? string.Empty;

    public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";

    public string Company => User?.FindFirst("Company")?.Value ?? string.Empty;

    public string Dataset => User?.FindFirst("Dataset")?.Value ?? string.Empty;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
