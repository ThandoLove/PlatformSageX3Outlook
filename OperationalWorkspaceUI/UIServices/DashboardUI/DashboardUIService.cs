
// CODE START: Services/Dashboard/DashboardUIService.cs

using OperationalWorkspace.Models.Dashboard;
using OperationalWorkspaceUI.State;

namespace OperationalWorkspace.UIServices.DashboardUI;

public class DashboardUIService
{
    public async Task LoadDashboardAsync(DashboardState state)
    {
        // Example: fetch dashboard data from API or local cache
        // For production: inject HttpClient or API client
        state.ERPData = await Task.FromResult(GetERPData());
        state.CRMData = await Task.FromResult(GetCRMData());
        state.Tasks = await Task.FromResult(GetTasks());
        state.FinanceData = await Task.FromResult(GetFinanceData());
        state.SystemHealth = await Task.FromResult(GetSystemHealth());
        state.RecentActivity = await Task.FromResult(GetRecentActivity());
        state.KnowledgeBase = await Task.FromResult(GetKnowledgeBase());
        state.AuditLogs = await Task.FromResult(GetAuditLogs());
    }

    private object GetERPData() => new { Orders = 123, Revenue = 45678 };
    private object GetCRMData() => new { TopClients = 12, Opportunities = 7 };
    private object GetTasks() => new List<object> { new { Title = "Task 1", Status = "Open" } };
    private object GetFinanceData() => new { Receivables = 10234, Payables = 2045 };
    private object GetSystemHealth() => new { Uptime = "99.9%", Errors = 0 };
    private object GetRecentActivity() => new List<object> { new { User = "Alice", Action = "Created Order" } };
    private object GetKnowledgeBase() => new List<object> { new { Title = "KB Article", Link = "#" } };
    private object GetAuditLogs() => new List<object> { new { Event = "Login", User = "Bob", Time = DateTime.Now } };
}
// CODE END