using OperationalWorkspaceUI.Models.Dashboard;

namespace OperationalWorkspaceUI.Models.Email
{
    // CODE START
    public class EmailContextModel
    {
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public BusinessPartnerLookupModel BusinessPartner { get; set; } = new BusinessPartnerLookupModel();

        public List<OrderSummaryModel> Orders { get; set; } = new List<OrderSummaryModel>();
        public List<TaskItemModel> Tasks { get; set; } = new List<TaskItemModel>();
    }
    // CODE END
}
