namespace OperationalWorkspace.Domain.Entities;

/// <summary>
/// ERP-grade audit log entry (Sage X3 + Outlook + Blazor + API)
/// </summary>
public sealed class AuditLogEntry
{
    // =========================
    // PRIMARY ID
    // =========================
    public Guid Id { get; init; } = Guid.NewGuid();

    // =========================
    // EVENT INFO
    // =========================
    public string EventType { get; init; } = string.Empty;        // Create / Update / Delete / Login
    public string EventCategory { get; init; } = string.Empty;     // CRM / ERP / Finance / Inventory

    public string Description { get; init; } = string.Empty;

    // =========================
    // ENTITY CONTEXT
    // =========================
    public string EntityName { get; init; } = string.Empty;        // Invoice, Order, Contact
    public Guid? EntityId { get; init; }

    // =========================
    // CHANGE TRACKING
    // =========================
    public string? OldValuesJson { get; init; }
    public string? NewValuesJson { get; init; }

    // =========================
    // USER CONTEXT
    // =========================
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserRole { get; init; }

    // =========================
    // TENANT CONTEXT (IMPORTANT FOR YOU)
    // =========================
    public string? Company { get; init; }
    public string? Dataset { get; init; }

    // =========================
    // SYSTEM CONTEXT
    // =========================
    public string? SourceSystem { get; init; }   // Outlook / Blazor / API / SageX3
    public string? CorrelationId { get; init; }

    // HTTP CONTEXT (VERY IMPORTANT)
    public string? Path { get; init; }
    public string? Method { get; init; }
    public string? IpAddress { get; init; }

    // =========================
    // SEVERITY
    // =========================
    public string Severity { get; init; } = "Info";

    // =========================
    // TIMESTAMP (ONLY ONE!)
    // =========================
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;

    public string? TraceId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public Guid? PerformedByUserId { get; set; }


}