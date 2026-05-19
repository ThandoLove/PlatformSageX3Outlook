
namespace OperationalWorkspaceApplication.DTOs;

public class AuthTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}