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

    public async Task<List<ActivityDto>> GetRecentActivityForUserAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid)) return new List<ActivityDto>();

        var logs = await _repo.GetByUserAsync(userGuid, DateTime.UtcNow.AddDays(-7));

        return logs.Select(l => new ActivityDto(
            l.Id,
            l.EntityName,
            l.Description,
            l.EventType,
            l.EntityId ?? Guid.Empty,
            l.OccurredAtUtc,
            l.PerformedByUserName ?? "System",
            l.OccurredAtUtc,
            l.EventType
        )).ToList();
    }

    // 1. Implementation for GetAllAuditLogsAsync (Interface Member)
    public async Task<List<AuditLogDto>> GetAllAuditLogsAsync()
    {
        var logs = await _repo.GetByEventTypeAsync("ALL", DateTime.UtcNow.AddDays(-30)); // Longer history

        return logs.Select(l => new AuditLogDto
        {
            User = l.PerformedByUserName ?? "System",
            Action = l.EventType,
            Entity = l.EntityName,
            Timestamp = l.OccurredAtUtc
        }).ToList();
    }

    // 2. Implementation for GetAllRecentLogsAsync (Interface Member)
    // This fixes the 'does not implement' error you just received
    public async Task<List<AuditLogDto>> GetAllRecentLogsAsync()
    {
        var logs = await _repo.GetByEventTypeAsync("ALL", DateTime.UtcNow.AddDays(-1)); // Just last 24h

        return logs.Select(l => new AuditLogDto
        {
            User = l.PerformedByUserName ?? "System",
            Action = l.EventType,
            Entity = l.EntityName,
            Timestamp = l.OccurredAtUtc
        }).ToList();
    }
}
