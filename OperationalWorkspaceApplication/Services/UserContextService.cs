using Microsoft.AspNetCore.Http;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System.Security.Claims;
using System.Threading.Tasks;

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

        return Task.FromResult(new UserDto
        {
            Id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0",
            Name = user?.Identity?.Name ?? "Anonymous",
            Role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "None",
            Environment = "Production"
        });
    }

    // FIX: Added the missing interface member
    public Task<UserDto> GetUserAsync(string userId)
    {
        // For now, return a placeholder or implement your DB lookup logic here
        return Task.FromResult(new UserDto
        {
            Id = userId,
            Name = "System Looked-up User",
            Role = "User",
            Environment = "Production"
        });
    }
}
