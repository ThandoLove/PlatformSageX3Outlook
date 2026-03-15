using Microsoft.EntityFrameworkCore;
using OperationalWorkspaceApplication.Audit;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Abstractions;
using Task = System.Threading.Tasks.Task;
// This alias only works if OperationalWorkspace.Domain.Entities.AuditLog exists!
using AuditLogEntry = OperationalWorkspace.Domain.Entities.AuditLog;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly IntegrationDbContext _db;
    public AuditLogRepository(IntegrationDbContext db) => _db = db;

    public async Task AddAsync(AuditLogEntry entry, CancellationToken ct)
    {
        await _db.AuditLogs.AddAsync(entry, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<AuditLogEntry> entries, CancellationToken ct)
    {
        await _db.AuditLogs.AddRangeAsync(entries, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<AuditLogEntry?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.AuditLogs.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<AuditLogEntry>> GetByEntityAsync(string entityName, Guid entityId, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
    {
        // Use .ToString() if EntityId in the database is a string
        return await _db.AuditLogs
            .Where(x => x.EntityType == entityName && x.EntityId == entityId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AuditLogEntry>> GetByUserAsync(Guid userId, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
        => await _db.AuditLogs.Where(x => x.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<AuditLogEntry>> GetByEventTypeAsync(string eventType, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
        => await _db.AuditLogs.Where(x => x.EventType == eventType).ToListAsync(ct);

    public async Task<PagedResult<AuditLogEntry>> SearchAsync(AuditLogSearchCriteria criteria, CancellationToken ct)
    {
        var query = _db.AuditLogs.AsQueryable();
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((criteria.PageNumber - 1) * criteria.PageSize)
                               .Take(criteria.PageSize)
                               .ToListAsync(ct);

        // If the 4-argument constructor is missing, use object initializer:
        return new PagedResult<AuditLogEntry>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = criteria.PageNumber,
            PageSize = criteria.PageSize
        };
    }

    public Task AddAsync(OperationalWorkspaceApplication.Audit.AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<OperationalWorkspaceApplication.Audit.AuditLogEntry> entries, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    Task<OperationalWorkspaceApplication.Audit.AuditLogEntry?> IAuditLogRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<OperationalWorkspaceApplication.Audit.AuditLogEntry>> IAuditLogRepository.GetByEntityAsync(string entityName, Guid entityId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<OperationalWorkspaceApplication.Audit.AuditLogEntry>> IAuditLogRepository.GetByUserAsync(Guid userId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<OperationalWorkspaceApplication.Audit.AuditLogEntry>> IAuditLogRepository.GetByEventTypeAsync(string eventType, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<PagedResult<OperationalWorkspaceApplication.Audit.AuditLogEntry>> IAuditLogRepository.SearchAsync(AuditLogSearchCriteria criteria, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
