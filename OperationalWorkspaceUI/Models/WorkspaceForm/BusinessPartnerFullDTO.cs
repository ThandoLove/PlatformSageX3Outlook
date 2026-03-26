using OperationalWorkspaceUI.Models.Dashboard;
using OperationalWorkspaceUI.Models.Email;

namespace OperationalWorkspaceUI.Models.WorkspaceForm
{
    // CODE START
    public class BusinessPartnerFullDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<OrderSummaryDTO> Orders { get; set; } = new List<OrderSummaryDTO>();
        public List<TaskItemDTO> Tasks { get; set; } = new List<TaskItemDTO>();
        public List<ActivityItemDTO> Activities { get; set; } = new List<ActivityItemDTO>();
    }
    // CODE END
}
