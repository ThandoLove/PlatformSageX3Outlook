using Microsoft.Extensions.Logging;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceApplication.Services;

public sealed class ActivityService : IActivityService
{
    private readonly IActivityRepository _repo;
    private readonly ILogger<ActivityService> _logger;

    public ActivityService(IActivityRepository repo, ILogger<ActivityService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail)
    {
        var entity = new Activity
        {
            Title = dto.Title,
            Description = dto.Description,
            ActivityType = dto.ActivityType,
            // Convert Guid? to Guid by providing a fallback (Guid.Empty)
            RelatedEntityId = dto.RelatedEntityId ?? Guid.Empty,
            CreatedBy = userEmail,
            CreatedAt = DateTime.UtcNow,
            Timestamp = DateTime.UtcNow,
            Action = "Create"
        };

        await _repo.AddAsync(entity, default);

        return new ActivityDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.ActivityType,
            entity.RelatedEntityId ?? Guid.Empty, // Now passed as a non-nullable Guid
            entity.CreatedAt,
            entity.CreatedBy,
            entity.Timestamp,
            entity.Action);
    }

    public async Task<ActivityDto?> GetByIdAsync(Guid id)
    {
        var a = await _repo.GetByIdAsync(id, default);
        if (a == null) return null;

        return new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty, // Handle potential null from DB
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action);
    }

    public async Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId)
    {
        var activities = await _repo.GetByRelatedEntityAsync(partnerId, default);

        return activities.Select(a => new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty, // Handle potential null from DB
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action)).ToList();
    }

}
