using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
    public class WorkspaceState
    {
        public List<ClientDto> Clients { get; set; } = new();
        public List<OrderDto> Orders { get; set; } = new();
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

        public async Task LoadTasksAsync()
        {
            await Task.Delay(200);

            Tasks = new List<TaskDto>
    {
        new TaskDto
        {
            Title = "Review Invoice Sync",
            DueDate = DateTime.UtcNow.AddDays(1),
            Completed = false,
            CompanyName = "Demo Company",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        },
        new TaskDto
        {
            Title = "Fix Outlook Integration",
            DueDate = DateTime.UtcNow.AddDays(-1),
            Completed = false,
            CompanyName = "Sage X3",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        }
    };
        }

        public void ReloadClients() { }
        public void ReloadOrders() { }
        public void ReloadTasks() { }
    }
}