using OperationalWorkspaceUI.Models.Dashboard;

namespace OperationalWorkspaceUI.Models.Email
{
    // CODE START
    public class EmailContextDTO
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }

        public BusinessPartnerLookupDTO BusinessPartner { get; set; }

        public List<OrderSummaryDTO> Orders { get; set; }
        public List<TaskItemDTO> Tasks { get; set; }
    }
    // CODE END
}
