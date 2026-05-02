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
            state.AllTasks = await FetchTasksAsync(userRole);
            state.RecentActivities = await FetchActivitiesAsync();
            state.AllTickets = await FetchTicketsAsync(userRole);

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

        state.AdminErp = new AdminErpDto();
        state.AdminHealth = new AdminSystemHealthDto();
        state.AdminFinance = new AdminFinanceDto();
        state.AuditLogs = new List<AuditLogDto>();
    }

    private async Task<List<TicketDto>> FetchTicketsAsync(string role)
    {
        return new List<TicketDto>
        {
            new TicketDto
            {
                Id = Guid.NewGuid(),
                TicketNumber = "TK-001",
                Title = "Login Issue - Syracuse",
                AssignedTo = "CurrentUser",
                Priority = "5",
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new TicketDto
            {
                Id = Guid.NewGuid(),
                TicketNumber = "TK-002",
                Title = "Invoice Sync Error",
                AssignedTo = "CurrentUser",
                Priority = "3",
                CreatedAt = DateTime.Now
            }
        };
    }

    private async Task<List<TaskDto>> FetchTasksAsync(string role)
    {
        return new List<TaskDto>
        {
            new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "Review Sales Report",
                AssignedTo = "Admin",
                DueDate = DateTime.Now.AddDays(2),
                Completed = false
            },
            new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "Update Client Contact",
                AssignedTo = "CurrentUser",
                DueDate = DateTime.Now.AddDays(1),
                Completed = false
            }
        };
    }

    private async Task<List<ActivityDto>> FetchActivitiesAsync()
    {
        return new List<ActivityDto>
        {
            new ActivityDto
            {
                Title = "Sales Order Created",
                Action = "Created",
                Timestamp = DateTime.Now
            },
            new ActivityDto
            {
                Title = "Ticket #101",
                Action = "Updated",
                Timestamp = DateTime.Now.AddHours(-1)
            }
        };
    }

    private async Task<List<AuditLogDto>> FetchAuditLogsAsync()
    {
        return new List<AuditLogDto>
        {
            new AuditLogDto
            {
                User = "Admin",
                Action = "User Login",
                Entity = "Auth",
                Timestamp = DateTime.Now
            },
            new AuditLogDto
            {
                User = "Manager",
                Action = "Invoice Deleted",
                Entity = "Invoice",
                Timestamp = DateTime.Now.AddMinutes(-30)
            }
        };
    }
}