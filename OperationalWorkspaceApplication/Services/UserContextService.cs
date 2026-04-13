using Microsoft.AspNetCore.Http;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<UserDto> GetCurrentUserAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        // Try to get custom Sage claims from the JWT
        var environment = user?.FindFirst("sage_env")?.Value ?? "Production";
        var tenantId = user?.FindFirst("tenant_id")?.Value;

        return Task.FromResult(new UserDto
        {
            Id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0",
            Name = user?.Identity?.Name ?? "Anonymous",
            Role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "None",
            Environment = environment,
            TenantId = tenantId
        });
    }

    public Task<UserDto> GetUserAsync(string userId)
    {
        // Placeholder for DB lookup logic if needed later
        return Task.FromResult(new UserDto
        {
            Id = userId,
            Name = "System Looked-up User",
            Role = "User",
            Environment = "Production"
        });
    }
}
