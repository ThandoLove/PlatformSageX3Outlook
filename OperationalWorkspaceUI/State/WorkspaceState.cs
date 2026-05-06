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
        // 1. DATA PROPERTIES
        public List<ClientDto> Clients { get; set; } = new();
        public List<ContactCreateDto> Contacts { get; set; } = new();
        public List<OrderDto> Orders { get; set; } = new();
        public List<TaskDto> Tasks { get; set; } = new();
        public List<ActivityDto> ActivityLogs { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public List<string> Knowledge { get; set; } = new();
        public List<OpenOrderDto> Quotes { get; set; } = new();
        public EmailInsightDto? SelectedEmail { get; set; }

        // 2. STATE NOTIFICATION (Broadcast)
        public event Action? OnChange;
        public void Notify() => OnChange?.Invoke();

        // 3. LOGGING LOGIC
        public void LogActivity(string title, string action, string user = "Current User")
        {
            var newLog = new ActivityDto
            {
                Id = Guid.NewGuid(),
                Title = title,
                Action = action,
                CreatedBy = user,
                Timestamp = DateTime.Now
            };

            ActivityLogs.Insert(0, newLog);
            Notify();
        }

        // 4. DATA RELOAD METHODS
        public void ReloadKnowledge(List<string> articles) => Knowledge = articles;
        public void ReloadContacts(List<ContactCreateDto> contacts) => Contacts = contacts;
        public void ReloadClients() { }
        public void ReloadOrders() { }
        public void ReloadTasks() { }

        public async Task LoadTasksAsync()
        {
            await Task.Delay(200);

            Tasks = new List<TaskDto>
            {
                new TaskDto { Id = Guid.NewGuid(), Title = "Inquiry about Product XYZ", CompanyName = "Tech Innovations Inc.", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Setup Sage X3 Connector", CompanyName = "Sage Internal", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Review Sales Forecast Q3", CompanyName = "Global Systems", Priority = TaskPriority.High, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Verify Invoice #8821", CompanyName = "Brightwave", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Submit Expense Report", CompanyName = "Finance Dept", Priority = TaskPriority.High, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Daily Team Sync", CompanyName = "Operational Team", Priority = TaskPriority.Medium, DueDate = DateTime.Today, Status = TaskStatus.Open }
            };
            Notify();
        }
    }
}
