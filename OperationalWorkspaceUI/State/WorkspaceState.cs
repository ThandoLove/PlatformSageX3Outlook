using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceUI.State
{
    public class WorkspaceState
    {
        // Core collections
        public List<ClientDto> Clients { get; set; } = new();
        public List<OrderDto> Orders { get; set; } = new();

        // FIXED: Removed the duplicate "Tasks" property that was causing the error
        public List<TaskDto> Tasks { get; set; } = new();

        public List<ActivityDto> ActivityLogs { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();

        public List<string> Knowledge { get; set; } = new();
        public List<OpenOrderDto> Quotes { get; set; } = new();
        public EmailInsightDto? SelectedEmail { get; set; }

        public void ReloadKnowledge(List<string> articles)
        {
            Knowledge = articles;
        }

        /// <summary>
        /// Loads the specific dummy data matching the high-fidelity mockup.
        /// This ensures the Alex Turner view is populated correctly.
        /// </summary>
        public async Task LoadTasksAsync()
        {
            // Simulate network latency
            await Task.Delay(200);

            Tasks = new List<TaskDto>
            {
                new TaskDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Inquiry about Product XYZ",
                    CompanyName = "Tech Innovations Inc.",
                    Priority = TaskPriority.Urgent,
                    DueDate = DateTime.Today,
                    Status = TaskStatus.Open,
                    Description = "Follow up on bulk purchase inquiry from Sage X3 lead.",
                    OutlookItemId = "AAMkAGEx..."
                },
                new TaskDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Develop QTE-7823 for Client B",
                    CompanyName = "Atlas Co.",
                    Priority = TaskPriority.High,
                    DueDate = DateTime.Today.AddDays(-1),
                    Status = TaskStatus.Open,
                    Description = "Quote development required for Atlas Co expansion."
                },
                new TaskDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Fix Bug in Client Portal",
                    CompanyName = "Brightwave Solutions",
                    Priority = TaskPriority.Medium,
                    DueDate = DateTime.Today.AddDays(2),
                    Status = TaskStatus.Assigned
                },
                new TaskDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Setup Sage X3 Connector",
                    CompanyName = "Sage Internal",
                    Priority = TaskPriority.Urgent,
                    DueDate = DateTime.Today,
                    Status = TaskStatus.Open
                },
                new TaskDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Invoice Verification #459",
                    CompanyName = "Greenfield Corp",
                    Priority = TaskPriority.Low,
                    DueDate = DateTime.Today.AddDays(1),
                    Status = TaskStatus.Completed
                }
            };
        }

        public void ReloadClients() { }
        public void ReloadOrders() { }
        public void ReloadTasks() { }
    }
}
