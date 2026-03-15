namespace OperationalWorkspace.Domain.Entities;

using OperationalWorkspace.Domain.Enums;

public record AuditLog(
    string EntityName,
    Guid EntityId,
    AuditAction Action,
    string UserRole)
{
    // Properties used by the Repository and Interface
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Details { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public string EntityType { get; set; } = null!;
    // REMOVED duplicate EntityId here
    public string EventType { get; set; } = null!;
    public Guid UserId { get; set; }
}
