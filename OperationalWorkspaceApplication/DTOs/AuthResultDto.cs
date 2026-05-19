
namespace OperationalWorkspaceApplication.DTOs;

public class AuthResultDto
{
    // ======================================================
    // SUCCESS
    // ======================================================

    public bool Success { get; set; }

    // ======================================================
    // ACCESS TOKEN
    // ======================================================

    public string AccessToken { get; set; } = string.Empty;

    // ======================================================
    // REFRESH TOKEN
    // ======================================================

    public string? RefreshToken { get; set; }

    // ======================================================
    // EXPIRATION
    // ======================================================

    public DateTime ExpiresAtUtc { get; set; }

    // ======================================================
    // ERROR
    // ======================================================

    public string? ErrorMessage { get; set; }
}