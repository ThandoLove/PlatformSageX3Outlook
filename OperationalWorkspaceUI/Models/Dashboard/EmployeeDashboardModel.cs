namespace OperationalWorkspaceUI.Models.Dashboard;

// CODE START
public class EmployeeDashboardModel
{
    public ERPOverviewModel ERP { get; set; } = new ERPOverviewModel();
    public CRMOverviewModel CRM { get; set; } = new CRMOverviewModel();
    public List<TaskItemModel> Tasks { get; set; } = new List<TaskItemModel>();
    public FinanceSummaryModel Finance { get; set; } = new FinanceSummaryModel();
    public List<ActivityItemModel> RecentActivity { get; set; } = new List<ActivityItemModel>();
}
// CODE END