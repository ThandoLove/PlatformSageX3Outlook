namespace OperationalWorkspaceUI.Models.Dashboard;

// CODE START
public class EmployeeDashboardDTO
{
    public ERPOverviewDTO ERP { get; set; } = new ERPOverviewDTO();
    public CRMOverviewDTO CRM { get; set; } = new CRMOverviewDTO();
    public List<TaskItemDTO> Tasks { get; set; } = new List<TaskItemDTO>();
    public FinanceSummaryDTO Finance { get; set; } = new FinanceSummaryDTO();
    public List<ActivityItemDTO> RecentActivity { get; set; } = new List<ActivityItemDTO>();
}
// CODE END