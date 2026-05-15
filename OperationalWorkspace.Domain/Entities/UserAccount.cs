
using System;

namespace OperationalWorkspace.Domain.Entities;

public class UserAccount
{
    public Guid Id { get; set; }

    // LOGIN IDENTITY
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // SECURITY
    public string PasswordHash { get; set; } = string.Empty;

    // ROLE SYSTEM
    public string Role { get; set; } = "User";

    // LOCKOUT SECURITY
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockoutEnd { get; set; }

    // AUDIT
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // OPTIONAL (useful for Sage X3 / Blazor / Outlook integration)
    public bool IsActive { get; set; } = true;
}