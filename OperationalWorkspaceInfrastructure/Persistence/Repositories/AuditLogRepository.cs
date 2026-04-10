using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Audit;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.IServices;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly IntegrationDbContext _db;

    public AuditLogRepository(IntegrationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLogEntry entry, CancellationToken ct = default)
    {
        await _db.AuditLogs.AddAsync(entry, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<AuditLogEntry> entries, CancellationToken ct = default)
    {
        await _db.AuditLogs.AddRangeAsync(entries, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<AuditLogEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AuditLogs.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<AuditLogEntry>> GetByEntityAsync(string entityName, Guid entityId, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
    {
        return await _db.AuditLogs
            .Where(x => x.EntityName == entityName && x.EntityId == entityId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AuditLogEntry>> GetByUserAsync(Guid userId, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
        => await _db.AuditLogs.Where(x => x.PerformedByUserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<AuditLogEntry>> GetByEventTypeAsync(string eventType, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
        => await _db.AuditLogs.Where(x => x.EventType == eventType).ToListAsync(ct);

    public async Task<PagedResult<AuditLogEntry>> SearchAsync(AuditLogSearchCriteria criteria, CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();

        // Optional: Add basic filtering based on search criteria
        if (!string.IsNullOrEmpty(criteria.EventType))
            query = query.Where(x => x.EventType == criteria.EventType);

        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((criteria.PageNumber - 1) * criteria.PageSize)
                               .Take(criteria.PageSize)
                               .ToListAsync(ct);

        return new PagedResult<AuditLogEntry>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = criteria.PageNumber,
            PageSize = criteria.PageSize
        };
    }
}
