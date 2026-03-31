using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspace.UIServices.DashboardUI;

namespace OperationalWorkspaceUI.State;

public class DashboardState
{
    private readonly DashboardUIService _dashboardService;

    public DashboardState(DashboardUIService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // --- ADMIN DASHBOARD DATA ---
    public AdminErpDto AdminErp { get;  set; } = new();
    public AdminCrmDto AdminCrm { get; set; } = new();
    public AdminFinanceDto AdminFinance { get; set; } = new();
    public AdminSystemHealthDto AdminHealth { get; set; } = new();
    public List<AuditLogDto> AuditLogs { get; set; } = new();

    // --- EMPLOYEE DASHBOARD DATA ---
    public EmployeeErpDto EmployeeErp { get; set; } = new();
    public EmployeeCRMDTO EmployeeCrm { get; set; } = new();
    public EmployeeFinanceDto EmployeeFinance { get; set; } = new();
    public List<TaskDto> MyTasks { get; set; } = new();

    // --- SHARED DATA ---
    public List<ClientDto> TopClients { get; set; } = new();
    public List<ActivityDto> RecentActivities { get; set; } = new();
    public List<TaskDto> AllTasks { get; set; } = new();

    // --- State Management ---
    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    // Inside DashboardState.cs, add this line with your other Admin/Shared data:
    public List<KnowledgeDto> KnowledgeBase { get; set; } = new();


    // --- Methods ---
    public async Task LoadAdminDashboardAsync()
    {
        await _dashboardService.LoadDashboardAsync(this);
        NotifyStateChanged();
    }

    public async Task LoadEmployeeDashboardAsync()
    {
        await _dashboardService.LoadDashboardAsync(this);
        // Logic to filter tasks for the current user
        MyTasks = AllTasks.Where(t => t.Status != "Completed").ToList();
        NotifyStateChanged();
    }
}
