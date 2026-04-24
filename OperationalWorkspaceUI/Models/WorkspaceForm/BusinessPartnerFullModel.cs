using OperationalWorkspaceUI.Models.Dashboard;
using OperationalWorkspaceUI.Models.Email;

namespace OperationalWorkspaceUI.Models.WorkspaceForm
{
    // CODE START
    public class BusinessPartnerFullModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<OrderSummaryModel> Orders { get; set; } = new List<OrderSummaryModel>();
        public List<TaskItemModel> Tasks { get; set; } = new List<TaskItemModel>();
        public List<ActivityItemModel> Activities { get; set; } = new List<ActivityItemModel>();
    }
    // CODE END
}
