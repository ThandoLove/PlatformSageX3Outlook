
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
    
    public class WorkspaceState
    {
        public List<ClientDTO> Clients { get; set; } = new();
        public List<OrderDTO> Orders { get; set; } = new();
        public List<TaskDto> Tasks { get; set; } = new();
        public List<ActivityDto> ActivityLogs { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public List<UserDTO> Users { get; set; } = new();

        public void PreFillOrderFromEmail(EmailInsightDto email)
        {
            Orders.Add(new OrderDTO
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
                Description = email.Body,
                AssignedToId = email.AssignedUserId,
                DueDate = DateTime.UtcNow.AddDays(3)
            });
        }

        public void ReloadClients() { /* Call UI service to reload */ }
        public void ReloadOrders() { /* Call UI service to reload */ }
        public void ReloadTasks() { /* Call UI service to reload */ }
    }
}
