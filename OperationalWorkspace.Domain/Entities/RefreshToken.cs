using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspace.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}