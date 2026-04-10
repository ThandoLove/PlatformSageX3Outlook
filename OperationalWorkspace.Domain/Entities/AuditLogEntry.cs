using System;

namespace OperationalWorkspace.Domain.Entities;

/// <summary>
/// Represents a comprehensive audit log entry for the Sage X3 Outlook Workspace.
/// Supports change tracking (JSON), user context, and system correlation.
/// </summary>
public sealed class AuditLogEntry
{
    // Primary Key
    public Guid Id { get; init; } = Guid.NewGuid();

    // ===== Event Information =====
    // e.g., "User Login", "Order Created", "Stock Check"
    public string EventType { get; init; } = string.Empty;

    // Financial | Inventory | Task | Security | Integration | System
    public string EventCategory { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    // ===== ERP Entity Context =====
    // e.g., "SalesOrder", "BusinessPartner", "Invoice"
    public string EntityName { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }

    // ===== Change Tracking (Crucial for Sage X3) =====
    public string? OldValuesJson { get; init; }
    public string? NewValuesJson { get; init; }

    // ===== User Context =====
    public Guid? PerformedByUserId { get; init; }
    public string? PerformedByUserName { get; init; }

    // ===== System/Request Context =====
    public string? CorrelationId { get; init; }

    // OutlookAddIn | SageX3API | Scheduler
    public string? SourceSystem { get; init; }

    // ===== Severity & Status =====
    // Info | Warning | Critical
    public string Severity { get; init; } = "Info";

    // ===== Timestamp =====
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
    public string EntityType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
