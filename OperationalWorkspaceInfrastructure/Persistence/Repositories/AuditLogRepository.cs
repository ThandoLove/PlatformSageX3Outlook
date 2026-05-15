using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Audit;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.IServices;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly IntegrationDbContext _db;

    public AuditLogRepository(IntegrationDbContext db)
    {
        _db = db;
    }

    // ==============================
    // WRITE
    // ==============================
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
    {
        return await _db.AuditLogs
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    // ==============================
    // ENTITY FILTER
    // ==============================
    public async Task<IReadOnlyList<AuditLogEntry>> GetByEntityAsync(
        string entityName,
        Guid entityId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();

        query = query.Where(x => x.EntityName == entityName && x.EntityId == entityId);

        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);

        return await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ToListAsync(ct);
    }

    // ==============================
    // USER FILTER
    // ==============================
    public async Task<IReadOnlyList<AuditLogEntry>> GetByUserAsync(
        Guid userId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();

        query = query.Where(x => x.PerformedByUserId == userId);

        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);

        return await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ToListAsync(ct);
    }

    // ==============================
    // EVENT TYPE FILTER
    // ==============================
    public async Task<IReadOnlyList<AuditLogEntry>> GetByEventTypeAsync(
        string eventType,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();

        query = query.Where(x => x.EventType == eventType);

        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);

        return await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ToListAsync(ct);
    }

    // ==============================
    // SEARCH (FULL POWER)
    // ==============================
    public async Task<PagedResult<AuditLogEntry>> SearchAsync(
        AuditLogSearchCriteria criteria,
        CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.EventType))
            query = query.Where(x => x.EventType == criteria.EventType);

        if (!string.IsNullOrWhiteSpace(criteria.EntityName))
            query = query.Where(x => x.EntityName == criteria.EntityName);

        if (criteria.EntityId.HasValue)
            query = query.Where(x => x.EntityId == criteria.EntityId);

        if (criteria.UserId.HasValue)
            query = query.Where(x => x.PerformedByUserId == criteria.UserId);

        if (criteria.FromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= criteria.FromUtc);

        if (criteria.ToUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= criteria.ToUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
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