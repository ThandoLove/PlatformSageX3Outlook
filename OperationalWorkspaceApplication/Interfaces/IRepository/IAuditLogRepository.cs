using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Abstractions; // Add this using
using OperationalWorkspaceApplication.Audit; // Add this using for AuditLogSearchCriteria
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IAuditLogRepository
{
    // Write
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<AuditLogEntry> entries, CancellationToken cancellationToken = default);

    // Read by Identity
    Task<AuditLogEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Filtered Reads
    Task<IReadOnlyList<AuditLogEntry>> GetByEntityAsync(
        string entityName, Guid entityId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLogEntry>> GetByUserAsync(
        Guid userId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLogEntry>> GetByEventTypeAsync(
        string eventType, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default);

    // Advanced Search
    Task<PagedResult<AuditLogEntry>> SearchAsync(
        AuditLogSearchCriteria criteria, CancellationToken cancellationToken = default);
}
