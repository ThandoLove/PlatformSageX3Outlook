using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
   
    public class EmailContextState
    {
        public EmailInsightDto? CurrentEmail { get; set; }
        public ClientDto? MatchedClient { get; set; }
        public List<OrderDto> LinkedOrders { get; set; } = new();
        public List<TaskDto> LinkedTasks { get; set; } = new();

        // Optional: the raw initial mailbox item loaded from Office.js
        public string InitialSubject { get; set; } = string.Empty;
        public string InitialBody { get; set; } = string.Empty;
        public string InitialFrom { get; set; } = string.Empty;
    }
}
