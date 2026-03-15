using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Audit;


public sealed class AuditLogSearchCriteria
{
    public string? EventType { get; init; }
    public string? EventCategory { get; init; }

    public string? EntityName { get; init; }
    public Guid? EntityId { get; init; }

    public Guid? UserId { get; init; }

    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }

    public string? Severity { get; init; }

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}