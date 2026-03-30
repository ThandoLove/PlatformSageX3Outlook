using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
   
    public class EmailContextState
    {
        public EmailInsightDto? CurrentEmail { get; set; }
        public ClientDto? MatchedClient { get; set; }
        public List<OrderDto> LinkedOrders { get; set; } = new();
        public List<TaskDto> LinkedTasks { get; set; } = new();
    }
}
