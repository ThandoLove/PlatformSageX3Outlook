using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class ActivityUIService
    {
        // LOGIC: Use the DTO list to store structured data instead of plain strings
        private readonly List<ActivityDto> _activities = new();

        public Task<List<ActivityDto>> GetActivitiesAsync()
        {
            // Logic: Returns the DTO list expected by Activity.razor
            return Task.FromResult(_activities);
        }

        public Task LogActivityAsync(string title, string action, string createdBy)
        {
            // LOGIC: Create a structured DTO entry matching your Sage X3 requirement
            _activities.Add(new ActivityDto
            {
                Title = title,
                Action = action,
                CreatedBy = createdBy,
                Timestamp = DateTime.Now
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Mock method to populate initial data for testing.
        /// </summary>
        public void SeedInitialData()
        {
            _activities.Add(new ActivityDto { Title = "Sales Order Created", Action = "Created", CreatedBy = "Admin", Timestamp = DateTime.Now.AddHours(-2) });
            _activities.Add(new ActivityDto { Title = "Customer Follow-up", Action = "Call", CreatedBy = "John Smith", Timestamp = DateTime.Now.AddDays(-1) });
        }
    }
}
