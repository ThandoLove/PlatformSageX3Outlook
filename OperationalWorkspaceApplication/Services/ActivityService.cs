using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    // FETCH ACTIONS: Loads global database records up to the tracking layout grids
    public async System.Threading.Tasks.Task<IEnumerable<ActivityDto>> GetActivitiesAsync()
    {
        var activities = await _repo.GetAllAsync(default);

        return activities.Select(a => new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty,
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action)).ToList();
    }

    public async System.Threading.Tasks.Task<ActivityDto?> GetByIdAsync(Guid id)
    {
        var a = await _repo.GetByIdAsync(id, default);
        if (a == null) return null;

        return new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty,
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action);
    }

    public async System.Threading.Tasks.Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId)
    {
        var activities = await _repo.GetByRelatedEntityAsync(partnerId, default);

        return activities.Select(a => new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty,
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action)).ToList();
    }
    public async System.Threading.Tasks.Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail)
    {
        var entity = new Activity
        {
            Title = dto.Title,
            Description = dto.Description,
            ActivityType = dto.ActivityType,
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
            entity.RelatedEntityId ?? Guid.Empty,
            entity.CreatedAt,
            entity.CreatedBy,
            entity.Timestamp,
            entity.Action);
    }

    public async System.Threading.Tasks.Task AttachEmailAsync(object model)
    {
        _logger.LogInformation("Attaching email to activity context.");
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task LogAsync(ActivityDto activity)
    {
        _logger.LogInformation("Logging activity: {Title}", activity.Title);

        var entity = new Activity
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            ActivityType = activity.ActivityType,
            RelatedEntityId = activity.RelatedEntityId,
            CreatedBy = activity.CreatedBy,
            CreatedAt = activity.CreatedAt,
            Timestamp = activity.Timestamp,
            Action = activity.Action
        };

        await _repo.AddAsync(entity, default);
    }

    // 🚀 NEW INTERACTIVE ENGINE METHOD: DIRECT PERSISTENCE TO REPOSITORY DATABASE HOOK
    // This allows components to instantly save and audit real-time clicks session states!
    public async System.Threading.Tasks.Task LogSystemActionAsync(string subsystem, string textDescription, string actionType, string userEmail)
    {
        _logger.LogInformation("System tracking execution intercepted: [{Subsystem}] - {Description}", subsystem, textDescription);

        var persistenceEntity = new Activity
        {
            Id = Guid.NewGuid(),
            Title = subsystem,             // Maps category like "Contact" or "Ticket" to table header
            Description = textDescription, // Detailed string event logs data
            ActivityType = "SystemTrace",
            RelatedEntityId = Guid.Empty,
            CreatedBy = userEmail,
            CreatedAt = DateTime.UtcNow,
            Timestamp = DateTime.UtcNow,
            Action = actionType            // e.g., "Refresh", "View", "Update"
        };

        await _repo.AddAsync(persistenceEntity, default);
    }
}
