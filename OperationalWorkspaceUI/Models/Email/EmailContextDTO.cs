using OperationalWorkspaceUI.Models.Dashboard;

namespace OperationalWorkspaceUI.Models.Email
{
    // CODE START
    public class EmailContextDTO
    {
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public BusinessPartnerLookupDTO BusinessPartner { get; set; } = new BusinessPartnerLookupDTO();

        public List<OrderSummaryDTO> Orders { get; set; } = new List<OrderSummaryDTO>();
        public List<TaskItemDTO> Tasks { get; set; } = new List<TaskItemDTO>();
    }
    // CODE END
}
