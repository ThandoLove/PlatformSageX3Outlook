using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceUI.Models.Email;
using OperationalWorkspaceUI.UIServices.DashboardUI;
using OperationalWorkspaceUI.UIServices.System;
using OperationalWorkspaceUI.UIServices.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OperationalWorkspaceUI.Components.Pages.Welcome;

namespace OperationalWorkspaceUI.State
{
    public class DashboardState : IDisposable
    {
        private readonly DashboardUIService _dashboardService;
        private readonly IBusinessPartnerService _bpService;
        private readonly ActivityUIService _activityService;

        public DashboardState(
            DashboardUIService dashboardService,
            IBusinessPartnerService bpService,
            ActivityUIService activityService)
        {
            _dashboardService = dashboardService;
            _bpService = bpService;
            _activityService = activityService;
        }

        public bool IsAdminEnvironment { get; private set; }

        public void SetEnvironment(bool isAdmin)
        {
            if (IsAdminEnvironment != isAdmin)
            {
                IsAdminEnvironment = isAdmin;
                NotifyStateChanged();
            }
        }

        public TaskDto? SelectedTask { get; private set; }

        public void SetSelectedTask(TaskDto? task)
        {
            SelectedTask = task;
            NotifyStateChanged();
        }

        public string? SelectedOrderId { get; private set; }

        public async Task LoadOrderDetailsAsync(string? orderId)
        {
            SelectedOrderId = orderId;
            if (orderId != null)
            {
                EmailContext = new EmailInsightDto
                {
                    SenderName = "Sage X3",
                    Subject = $"Order: {orderId}",
                    ReceivedAt = DateTime.Now
                };
            }
            NotifyStateChanged();
            await Task.CompletedTask;
        }

        public List<TaskDto> AllTasks { get; set; } = new();
        public List<TaskDto> MyTasks { get; set; } = new();

        public List<TaskDto> CurrentTasks =>
            IsAdminEnvironment ? AllTasks : MyTasks;

        public List<TicketDto> AllTickets { get; set; } = new();
        public List<TicketDto> MyTickets { get; set; } = new();

        public List<TicketDto> CurrentTickets =>
            IsAdminEnvironment ? AllTickets : MyTickets;

        public List<ActivityDto> RecentActivities { get; set; } = new();
        public List<AuditLogDto> AuditLogs { get; set; } = new();
        public List<ActivityDto> CurrentActivities => RecentActivities;

        public List<AuditLogDto> CurrentAuditLogs => AuditLogs;
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<ReportDto> AvailableReports { get; set; } = new();
        public List<ClientDto> TopClients { get; set; } = new();
        public EmailInsightDto EmailContext { get; set; } = new();
        public DashboardDto DashboardData { get; set; } = new();
        public AdminErpDto AdminErp { get; set; } = new();
        public AdminCrmDto AdminCrm { get; set; } = new();
        public AdminFinanceDto AdminFinance { get; set; } = new();
        public AdminSystemHealthDto AdminHealth { get; set; } = new();
        public EmployeeErpDto EmployeeErp { get; set; } = new();
        public EmployeeCRMDTO EmployeeCrm { get; set; } = new();
        public EmployeeFinanceDto EmployeeFinance { get; set; } = new();

        public object CurrentCrm =>
            IsAdminEnvironment
            ? (object)AdminCrm
            : EmployeeCrm;

        public object CurrentFinance =>
            IsAdminEnvironment
            ? (object)AdminFinance
            : EmployeeFinance;

        public event Action? OnChange;

        public void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        public async Task LoadDashboardAsync(bool isAdmin = false)
        {
            try
            {
                IsAdminEnvironment = isAdmin;
                await _dashboardService.LoadDashboardAsync(this);
                RecentActivities = await _activityService.GetActivitiesAsync();

                if (IsAdminEnvironment)
                {
                    AdminHealth = await _dashboardService.GetSystemHealthAsync();
                    await LoadAdminSpecificDataAsync();
                }
                else
                {
                    AdminHealth = new AdminSystemHealthDto();
                }

                // FIX: Ensure mock data structures re-initialize on refresh if the service layer backend returns empty arrays
                EnsureDefaultAdminTasks();

                EnsureDefaultData();
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DashboardState Load Error: {ex.Message}");
            }
        }

        public void EnsureDefaultAdminTasks()
        {
            if (AllTasks == null || !AllTasks.Any())
            {
                AllTasks = new List<TaskDto> {
                    new TaskDto { Id = Guid.NewGuid(), Title = "Issue with Invoice INV-100123", CompanyName = "Brightwave Solutions", Priority = TaskPriority.Urgent, Status = OperationalWorkspace.Domain.Enums.TaskStatus.Assigned, AssignedTo = "Amit Patel", CreatedDate = new DateTime(2025, 5, 6, 9, 15, 0) },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Inquiry about Product XYZ", CompanyName = "Tech Innovations Inc.", Priority = TaskPriority.High, Status = OperationalWorkspace.Domain.Enums.TaskStatus.Open, AssignedTo = "Deepa Patel" },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Develop QTE-7823 Renewal", CompanyName = "Tech Innovations Inc.", Priority = TaskPriority.Medium, Status = OperationalWorkspace.Domain.Enums.TaskStatus.Assigned, AssignedTo = "John Smith" }
                };
            }
        }

        // EXPLICIT TRANSMISSION PORTAL: This is the ONLY context method allowed to assign a task from Admin over to Employee list
        public void SendTaskToEmployee(TaskDto task, string employeeEmail)
        {
            if (task == null) return;

            // Update parameters on the active card instance inside the Admin view
            task.AssignedTo = employeeEmail;

            if (MyTasks == null)
            {
                MyTasks = new List<TaskDto>();
            }

            // Verify if the employee's container list already has this task record to block double insertion bugs
            bool exists = MyTasks.Any(t => t.Id == task.Id || (t.Title == task.Title && t.CompanyName == task.CompanyName));

            if (!exists)
            {
                // Inject clean task instance into employee list track natively
                MyTasks.Insert(0, task);
            }

            NotifyStateChanged();
        }

        private async Task LoadAdminSpecificDataAsync()
        {
            if (!AuditLogs.Any())
            {
                AuditLogs = new List<AuditLogDto>
                {
                    new AuditLogDto
                    {
                        Action = "User Login",
                        Entity = "auth",
                        User = "Admin",
                        Timestamp = DateTime.Now.AddMinutes(-5)
                    },
                    new AuditLogDto
                    {
                        Action = "Invoice Generated",
                        Entity = "invoice",
                        User = "System",
                        Timestamp = DateTime.Now.AddHours(-1)
                    }
                };
            }
            await Task.CompletedTask;
        }

        private void EnsureDefaultData()
        {
            if (!KnowledgeBase.Any())
            {
                KnowledgeBase = new List<KnowledgeDto>
                {
                    new KnowledgeDto(Guid.NewGuid(), "Pricing Guide", "Content", "Sales", "Summary", "#"),
                    new KnowledgeDto(Guid.NewGuid(), "Shipping FAQ", "Content", "Support", "Summary", "#")
                };
            }
        }

        public void Dispose()
        {
            OnChange = null;
        }
    }
}
