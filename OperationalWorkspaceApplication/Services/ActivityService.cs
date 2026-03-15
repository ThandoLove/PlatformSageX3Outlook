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
            RelatedEntityId = dto.RelatedEntityId,
            CreatedBy = userEmail
        };

        await _repo.AddAsync(entity, default); // Shield: Using default CancellationToken
        return new ActivityDto(entity.Id, entity.Title, entity.Description, entity.ActivityType, entity.RelatedEntityId, entity.CreatedAt, entity.CreatedBy);
    }

    public async Task<ActivityDto?> GetByIdAsync(Guid id)
    {
        var a = await _repo.GetByIdAsync(id, default);
        return a == null ? null : new ActivityDto(a.Id, a.Title, a.Description, a.ActivityType, a.RelatedEntityId, a.CreatedAt, a.CreatedBy);
    }

    // FIX: Implementation of the missing Interface member
    // Production Shield: Maps Domain Entities to DTOs for the Blazor UI
    // FIX: This must match the Interface EXACTLY
    public async Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId)
    {
        var activities = await _repo.GetByRelatedEntityAsync(partnerId, default);

        // Return as a mapped list to satisfy the 'object' or 'IEnumerable' requirement
        return activities.Select(a => new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId,
            a.CreatedAt,
            a.CreatedBy)).ToList();
    }
}
