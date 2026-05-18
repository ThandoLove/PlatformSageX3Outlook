using OperationalWorkspaceAPI.AuditAPI;

namespace OperationalWorkspaceAPI.AuditAPI;

public sealed class AuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? UserId { get; set; }

    public string? Email { get; set; }

    public string? Company { get; set; }

    public string? Dataset { get; set; }

    public string? Source { get; set; }

    public string? HttpMethod { get; set; }

    public string? IpAddress { get; set; }

    public AuditEventType Action { get; set; }

    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime TimestampUtc { get; set; }
    public string Entity { get; internal set; } = string.Empty;
    public string EntityId { get; internal set; } = string.Empty;
}