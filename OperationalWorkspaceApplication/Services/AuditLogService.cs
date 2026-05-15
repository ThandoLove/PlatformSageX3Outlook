using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceApplication.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repo;

    public AuditLogService(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    // ==============================
    // REQUIRED: BULK LOGGING (FIX FOR YOUR ERROR)
    // ==============================
    public async Task LogBulkAsync(List<AuditLogEntry> entries)
    {
        await _repo.AddRangeAsync(entries);
    }

    // ==============================
    // USER ACTIVITY (LAST 7 DAYS)
    // ==============================
    public async Task<List<ActivityDto>> GetRecentActivityForUserAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return new List<ActivityDto>();

        var logs = await _repo.GetByUserAsync(
            userGuid,
            DateTime.UtcNow.AddDays(-7),
            DateTime.UtcNow);

        return logs.Select(l => new ActivityDto(
            l.Id,
            l.EntityName,
            l.Description,
            l.EventType,
            l.EntityId ?? Guid.Empty,
            l.OccurredAtUtc,
            l.UserName ?? "System",
            l.OccurredAtUtc,
            l.EventType
        )).ToList();
    }

    // ==============================
    // FULL AUDIT LOG (LAST 30 DAYS)
    // ==============================
    public async Task<List<AuditLogDto>> GetAllAuditLogsAsync()
    {
        var logs = await _repo.GetByUserAsync(
            Guid.Empty,
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow);

        return logs.Select(l => new AuditLogDto
        {
            User = l.UserName ?? "System",
            Action = l.EventType,
            Entity = l.EntityName,
            Timestamp = l.OccurredAtUtc
        }).ToList();
    }

    // ==============================
    // RECENT ACTIVITY (LAST 24 HOURS)
    // ==============================
    public async Task<List<AuditLogDto>> GetAllRecentLogsAsync()
    {
        var logs = await _repo.GetByUserAsync(
            Guid.Empty,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow);

        return logs.Select(l => new AuditLogDto
        {
            User = l.UserName ?? "System",
            Action = l.EventType,
            Entity = l.EntityName,
            Timestamp = l.OccurredAtUtc
        }).ToList();
    }
}