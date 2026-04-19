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
        string userRole = state.IsAdminEnvironment ? "Admin" : "Employee";

        try
        {
            // 1. Fetch Shared Data
            state.AllTasks = await FetchTasksAsync(userRole);
            state.RecentActivities = await FetchActivitiesAsync();

            // 2. TICKETS REPLACEMENT: Fetching Tickets instead of Opportunities
            state.AllTickets = await FetchTicketsAsync(userRole);

            // 3. SECURE GATE: Load Role-Specific Data
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
            Console.WriteLine($"Error loading Sage X3 data: {ex.Message}");
        }
    }

    private async Task LoadAdminData(DashboardState state)
    {
        state.AdminErp = new AdminErpDto
        {
            TotalOrdersToday = 42,
            InvoicesGenerated = 15,
            StockAlerts = 3
        };

        // Exact match for the $12,500 in the original image
        state.AdminFinance = new AdminFinanceDto
        {
            TotalRevenue = 450000.00m,
            OverdueAmount = 12500.00m
        };

        state.AdminHealth = new AdminSystemHealthDto
        {
            SageX3Connected = true,
            APIHealthStatus = "Healthy"
        };

        state.AuditLogs = await FetchAuditLogsAsync();
    }

    private async Task LoadEmployeeData(DashboardState state)
    {
        state.EmployeeErp = new EmployeeErpDto
        {
            MyOpenOrders = 5,
            PendingDeliveries = 2
        };

        // Reset Admin-only collections
        state.AdminErp = new AdminErpDto();
        state.AdminHealth = new AdminSystemHealthDto();
        state.AuditLogs = new List<AuditLogDto>();
        state.AdminFinance = new AdminFinanceDto();
    }

    // --- Data Fetching Methods (Updated for Tickets) ---

    private async Task<List<TicketDto>> FetchTicketsAsync(string role)
    {
        // Mocking Sage X3 Ticket Data
        return new List<TicketDto>
        {
            new TicketDto { Id = 1, Title = "Login Issue - Syracuse", Status = "Open", AssignedTo = "CurrentUser" },
            new TicketDto { Id = 2, Title = "Invoice Sync Error", Status = "In Progress", AssignedTo = "CurrentUser" }
        };
    }

    private async Task<List<TaskDto>> FetchTasksAsync(string role) => new()
    {
        new TaskDto { Id = Guid.NewGuid(), Title = "Review Sales Report", Status = "Pending", AssignedTo = "Admin" },
        new TaskDto { Id = Guid.NewGuid(), Title = "Update Client Contact", Status = "Open", AssignedTo = "CurrentUser" }
    };

    private async Task<List<ActivityDto>> FetchActivitiesAsync() => new()
    {
        new ActivityDto { Title = "Sales Order Created", Action = "Created", Timestamp = DateTime.Now },
        new ActivityDto { Title = "Ticket #101", Action = "Updated", Timestamp = DateTime.Now.AddHours(-1) }
    };

    private async Task<List<AuditLogDto>> FetchAuditLogsAsync() => new()
    {
        new AuditLogDto { User = "Admin", Action = "User Login", Entity = "Auth", Timestamp = DateTime.Now },
        new AuditLogDto { User = "Manager", Action = "Invoice Deleted", Entity = "Invoice", Timestamp = DateTime.Now.AddMinutes(-30) }
    };
}
