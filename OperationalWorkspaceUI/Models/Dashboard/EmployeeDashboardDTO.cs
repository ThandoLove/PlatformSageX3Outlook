namespace OperationalWorkspaceUI.Models.Dashboard;

// CODE START
public class EmployeeDashboardDTO
{
    public ERPOverviewDTO ERP { get; set; }
    public CRMOverviewDTO CRM { get; set; }
    public List<TaskItemDTO> Tasks { get; set; }
    public FinanceSummaryDTO Finance { get; set; }
    public List<ActivityItemDTO> RecentActivity { get; set; }
}
// CODE END