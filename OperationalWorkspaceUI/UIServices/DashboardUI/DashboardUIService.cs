using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.UIServices.DashboardUI;

public class DashboardUIService
{
    public async Task LoadDashboardAsync(DashboardState state)
    {
        // Mocking data population - replace with real API calls later
        state.AdminErp = new AdminErpDto
        {
            TotalOrdersToday = 42,
            InvoicesGenerated = 15,
            StockAlerts = 3
        };

        state.EmployeeErp = new EmployeeErpDto
        {
            MyOpenOrders = 5,
            PendingDeliveries = 2
        };

        state.AdminHealth = new AdminSystemHealthDto
        {
            SageX3Connected = true,
            APIHealthStatus = "Healthy"
        };

        // Populate Lists
        state.AllTasks = GetMockTasks();
        state.RecentActivities = GetMockActivities();
        state.AuditLogs = new List<AuditLogDto>();

        await Task.CompletedTask;
    }

    private List<TaskDto> GetMockTasks() => new()
    {
        new TaskDto { Id = Guid.NewGuid(), Title = "Review Sales Report", Status = "Pending" },
        new TaskDto { Id = Guid.NewGuid(), Title = "Update Client Contact", Status = "Open" }
    };

    private List<ActivityDto> GetMockActivities() => new()
    {
        new ActivityDto { Title = "New Order", Action = "Created", Timestamp = DateTime.Now },
        new ActivityDto { Title = "Inventory", Action = "Updated", Timestamp = DateTime.Now.AddHours(-1) }
    };
}
