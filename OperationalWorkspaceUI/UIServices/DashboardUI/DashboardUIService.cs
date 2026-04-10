using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;
using System.Net.Http.Json;

namespace OperationalWorkspaceUI.UIServices.DashboardUI;

public class DashboardUIService
{
    private readonly HttpClient _httpClient;

    public DashboardUIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LoadDashboardAsync(DashboardState state)
    {
        // 1. Identify the user and their role (Admin vs Employee)
        // In a real scenario, this comes from your AuthService/JWT claims
        string userRole = state.IsAdminEnvironment ? "Admin" : "Employee";

        try
        {
            // 2. Fetch Shared Data (Available to everyone)
            state.AllTasks = await FetchTasksAsync(userRole);
            state.RecentActivities = await FetchActivitiesAsync();

            // 3. SECURE GATE: Only fetch Admin data if the environment/role matches
            if (state.IsAdminEnvironment)
            {
                await LoadAdminData(state);
            }
            else
            {
                await LoadEmployeeData(state);
            }
        }
        catch (Exception ex)
        {
            // Log error and notify UI State
            Console.WriteLine($"Error loading Sage X3 data: {ex.Message}");
        }
    }

    private async Task LoadAdminData(DashboardState state)
    {
        // REAL API CALL: Fetching sensitive financial/system data from Syracuse
        // var response = await _httpClient.GetFromJsonAsync<AdminErpDto>("api/sage/admin/erp-summary");

        state.AdminErp = new AdminErpDto
        {
            TotalOrdersToday = 42,
            InvoicesGenerated = 15, // Sensitive data
            StockAlerts = 3
        };

        state.AdminHealth = new AdminSystemHealthDto
        {
            SageX3Connected = true,
            APIHealthStatus = "Healthy"
        };

        // Populate Audit Logs only for Admins
        state.AuditLogs = await FetchAuditLogsAsync();
    }

    private async Task LoadEmployeeData(DashboardState state)
    {
        // REAL API CALL: Fetching only what the specific employee is allowed to see
        state.EmployeeErp = new EmployeeErpDto
        {
            MyOpenOrders = 5,
            PendingDeliveries = 2
        };

        // Explicitly clear sensitive admin data to ensure security in the UI state
        // Assign new empty instances instead of null to satisfy nullable reference rules
        state.AdminErp = new AdminErpDto();
        state.AdminHealth = new AdminSystemHealthDto();
        state.AuditLogs = new List<AuditLogDto>();
    }

    // --- Data Fetching Methods (Simulated API endpoints) ---

    private async Task<List<TaskDto>> FetchTasksAsync(string role)
    {
        // Logic: Admins see all tasks, Employees see assigned tasks
        return new List<TaskDto>
        {
            new TaskDto { Id = Guid.NewGuid(), Title = "Review Sales Report", Status = "Pending", AssignedTo = "Admin" },
            new TaskDto { Id = Guid.NewGuid(), Title = "Update Client Contact", Status = "Open", AssignedTo = "CurrentUser" }
        };
    }

    private async Task<List<ActivityDto>> FetchActivitiesAsync() => new()
    {
        new ActivityDto { Title = "New Order", Action = "Created", Timestamp = DateTime.Now },
        new ActivityDto { Title = "Inventory", Action = "Updated", Timestamp = DateTime.Now.AddHours(-1) }
    };

    private async Task<List<AuditLogDto>> FetchAuditLogsAsync() => new()
    {
        new AuditLogDto { User = "Admin", Action = "User Login", Entity = "Auth", Timestamp = DateTime.Now },
        new AuditLogDto { User = "Manager", Action = "Invoice Deleted", Entity = "Invoice", Timestamp = DateTime.Now.AddMinutes(-30) }
    };
}
