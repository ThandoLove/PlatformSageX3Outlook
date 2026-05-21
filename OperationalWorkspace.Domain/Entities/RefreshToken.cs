using System;

namespace OperationalWorkspace.Domain.Entities
{
    public class RefreshToken
    {
        // Your existing core database tracking columns (Preserved)
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Enterprise Auditing & Token-Reuse Security Protections (Added)
        public string JwtId { get; set; } = string.Empty; // Binds this refresh token cryptographically to its unique access token pair
        public bool IsUsed { get; set; } // Flag to detect and mitigate malicious token replay attempts
        public string CreatedByIp { get; set; } = string.Empty; // Traces origin network location for strict security audit trails
    }
}
