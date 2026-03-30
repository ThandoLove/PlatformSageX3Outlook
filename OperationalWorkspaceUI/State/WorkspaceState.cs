
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

        public void ReloadKnowledge(List<string> articles)
        {
            Knowledge = articles;
        }

        public void PreFillOrderFromEmail(EmailInsightDto email)
        {
            Orders.Add(new OrderDto
            {
                ClientId = email.ClientId,
                OrderDate = DateTime.UtcNow,
                Description = $"Auto-generated from email {email.Subject}"
            });
        }

        public void PreFillTaskFromEmail(EmailInsightDto email)
        {
            Tasks.Add(new TaskDto
            {
                Title = $"Follow-up: {email.Subject}",
                StatusDescription = email.Message,
                AssignedTo = email.AssignedUserId,
                DueDate = DateTime.UtcNow.AddDays(3)
            });
        }

        public void ReloadClients() { /* Call UI service to reload */ }
        public void ReloadOrders() { /* Call UI service to reload */ }
        public void ReloadTasks() { /* Call UI service to reload */ }
    }
}
