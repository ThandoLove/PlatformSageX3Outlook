using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
   
    public class EmailContextState
    {
        public EmailInsightDto CurrentEmail { get; set; }
        public ClientDTO MatchedClient { get; set; }
        public List<OrderDTO> LinkedOrders { get; set; } = new();
        public List<TaskDto> LinkedTasks { get; set; } = new();
    }
}
