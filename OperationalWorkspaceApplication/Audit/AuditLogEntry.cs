using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Audit;


public sealed class AuditLogEntry
{
    public Guid Id { get; init; }

    // ===== Event Info =====
    public string EventType { get; init; } = string.Empty;
    public string EventCategory { get; init; } = string.Empty;
    // Financial | Inventory | Task | Security | Integration | System

    public string Description { get; init; } = string.Empty;

    // ===== Entity Context =====
    public string EntityName { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }

    // ===== Change Tracking =====
    public string? OldValuesJson { get; init; }
    public string? NewValuesJson { get; init; }

    // ===== User Context =====
    public Guid? PerformedByUserId { get; init; }
    public string? PerformedByUserName { get; init; }

    // ===== Request Context =====
    public string? CorrelationId { get; init; }
    public string? SourceSystem { get; init; } // OutlookAddIn | ERP | API | Scheduler

    // ===== Severity =====
    public string Severity { get; init; } = "Info";
    // Info | Warning | Critical

    // ===== Timestamp =====
    public DateTime OccurredAtUtc { get; init; }
}