using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Added for Task support

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IActivityService
    {
        // ADD THIS: To fetch the list for the Activity Log page
        System.Threading.Tasks.Task<IEnumerable<ActivityDto>> GetActivitiesAsync();

        System.Threading.Tasks.Task<ActivityDto?> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail);
        System.Threading.Tasks.Task AttachEmailAsync(object model);
        System.Threading.Tasks.Task LogAsync(ActivityDto activity);
        System.Threading.Tasks.Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId);
    }
}
