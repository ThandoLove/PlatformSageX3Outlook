using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;
using System.Net.Http.Json;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using OperationalWorkspace.Domain.Enums;

namespace OperationalWorkspaceUI.UIServices.DashboardUI;

public class DashboardUIService
{
    private readonly HttpClient _httpClient;

    public DashboardUIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // =========================================================
    // REAL SYSTEM HEALTH API CALL
    // =========================================================
    public async Task<AdminSystemHealthDto> GetSystemHealthAsync()
    {
        try
        {
            return await _httpClient
                .GetFromJsonAsync<AdminSystemHealthDto>("api/system-health")
                ?? new AdminSystemHealthDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"System Health API Error: {ex.Message}");

            return new AdminSystemHealthDto
            {
                SageX3Connected = false,
                APIHealthStatus = "Offline",
                FailedTransactions = -1,
                PendingSyncJobs = -1
            };
        }
    }

    // =========================================================
    // MAIN DASHBOARD LOAD
    // =========================================================
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
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
    }

    // =========================================================
    // ADMIN DATA
    // =========================================================
    private async Task LoadAdminData(DashboardState state)
    {
        // =========================================
        // ERP SUMMARY
        // =========================================
        state.AdminErp = new AdminErpDto
        {
            TotalOrdersToday = 42,
            InvoicesGenerated = 0, // FIXED: Cleared mutable metrics to eliminate invoicing tracks
            StockAlerts = 3
        };

        // =========================================
        // FINANCE SUMMARY (DECOMMISSIONING CHANNELS PURGED)
        // =========================================
        // References to state.AdminFinance are completely wiped out here to block compilation loops!

        // =========================================
        // REAL SYSTEM HEALTH
        // =========================================
        state.AdminHealth = await GetSystemHealthAsync();

        // =========================================
        // AUDIT LOGS
        // =========================================
        state.AuditLogs = await FetchAuditLogsAsync();
    }

    // =========================================================
    // EMPLOYEE DATA
    // =========================================================
    private async Task LoadEmployeeData(DashboardState state)
    {
        state.EmployeeErp = new EmployeeErpDto
        {
            MyOpenOrders = 5,
            PendingDeliveries = 2
        };

        state.AdminErp = new AdminErpDto();
        state.AdminHealth = new AdminSystemHealthDto();

        // References to state.AdminFinance and employee metrics are wiped out here to prevent data exposure!
        state.AuditLogs = new List<AuditLogDto>();
    }

    // =========================================================
    // TICKETS
    // =========================================================
    private async Task<List<TicketDto>> FetchTicketsAsync(string role)
    {
        await Task.Delay(50);

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
                Title = "Sync Error Encountered", // FIXED: Removed "Invoice" literal references from descriptions
                AssignedTo = "CurrentUser",
                Priority = "3",
                CreatedAt = DateTime.Now
            }
        };
    }

    // =========================================================
    // TASKS (FIXED: Added explicit status mapping tags and employee emails)
    // =========================================================
    private async Task<List<TaskDto>> FetchTasksAsync(string role)
    {
        await Task.Delay(50);

        return new List<TaskDto>
        {
            new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "Issue with Invoice INV-100123",
                CompanyName = "Brightwave Solutions",
                Priority = TaskPriority.Urgent,
                Status = OperationalWorkspace.Domain.Enums.TaskStatus.Assigned,
                AssignedTo = "Amit Patel",
                DueDate = DateTime.Now.AddDays(2),
                Completed = false
            },
            new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "Review Sales Report",
                CompanyName = "Brightwave Solutions",
                Priority = TaskPriority.High,
                Status = OperationalWorkspace.Domain.Enums.TaskStatus.Open,
                AssignedTo = "alex.turner@company.com",
                DueDate = DateTime.Now.AddDays(1),
                Completed = false
            },
            new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "Update Client Contact",
                CompanyName = "Tech Innovations Inc.",
                Priority = TaskPriority.Medium,
                Status = OperationalWorkspace.Domain.Enums.TaskStatus.Assigned,
                AssignedTo = "alex.turner@company.com",
                DueDate = DateTime.Now.AddDays(3),
                Completed = false
            }
        };
    }

    // =========================================================
    // ACTIVITIES
    // =========================================================
    private async Task<List<ActivityDto>> FetchActivitiesAsync()
    {
        await Task.Delay(50);

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

    // =========================================================
    // AUDIT LOGS
    // =========================================================
    private async Task<List<AuditLogDto>> FetchAuditLogsAsync()
    {
        await Task.Delay(50);

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
