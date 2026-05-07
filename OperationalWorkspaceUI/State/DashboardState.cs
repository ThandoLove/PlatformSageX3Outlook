using OperationalWorkspaceUI.UIServices.DashboardUI;
using OperationalWorkspaceUI.UIServices.Workspace;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.Models.Email;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceUI.UIServices.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.State
{
    public class DashboardState
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

        // ==========================================
        // ENVIRONMENT & MODE (THE LINKING MECHANISM)
        // ==========================================
        public bool IsAdminEnvironment { get; private set; }

        public void SetEnvironment(bool isAdmin)
        {
            if (IsAdminEnvironment != isAdmin)
            {
                IsAdminEnvironment = isAdmin;
                NotifyStateChanged();
            }
        }

        // =========================
        // TASK SELECTION
        // =========================
        public TaskDto? SelectedTask { get; private set; }

        public void SetSelectedTask(TaskDto? task)
        {
            SelectedTask = task;
            NotifyStateChanged();
        }

        // =========================
        // ORDER CONTEXT
        // =========================
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

        // =========================
        // TASK DATA
        // =========================
        public List<TaskDto> AllTasks { get; set; } = new();
        public List<TaskDto> MyTasks { get; set; } = new();

        // This is the core logic: The UI binds to CurrentTasks, 
        // and this property decides which list to show.
        public List<TaskDto> CurrentTasks =>
            IsAdminEnvironment ? AllTasks : MyTasks;

        // =========================
        // TICKETS
        // =========================
        public List<TicketDto> AllTickets { get; set; } = new();
        public List<TicketDto> MyTickets { get; set; } = new();

        public List<TicketDto> CurrentTickets =>
            IsAdminEnvironment ? AllTickets : MyTickets;

        // =========================
        // ACTIVITY & AUDIT
        // =========================
        public List<ActivityDto> RecentActivities { get; set; } = new();
        public List<AuditLogDto> AuditLogs { get; set; } = new();
        public List<ActivityDto> CurrentActivities => RecentActivities;
        public List<AuditLogDto> CurrentAuditLogs => AuditLogs;

        // =========================
        // KNOWLEDGE & REPORTS
        // =========================
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<ReportDto> AvailableReports { get; set; } = new();
        public List<ClientDto> TopClients { get; set; } = new();

        // =========================
        // CONTEXTUAL DATA
        // =========================
        public EmailInsightDto EmailContext { get; set; } = new();
        public DashboardDto DashboardData { get; set; } = new();

        // =========================
        // CRM / ERP / FINANCE (Context Switchers)
        // =========================
        public AdminErpDto AdminErp { get; set; } = new();
        public AdminCrmDto AdminCrm { get; set; } = new();
        public AdminFinanceDto AdminFinance { get; set; } = new();
        public AdminSystemHealthDto AdminHealth { get; set; } = new();

        public EmployeeErpDto EmployeeErp { get; set; } = new();
        public EmployeeCRMDTO EmployeeCrm { get; set; } = new();
        public EmployeeFinanceDto EmployeeFinance { get; set; } = new();

        public object CurrentCrm =>
            IsAdminEnvironment ? (object)AdminCrm : EmployeeCrm;

        public object CurrentFinance =>
            IsAdminEnvironment ? (object)AdminFinance : EmployeeFinance;

        // =========================
        // EVENTS
        // =========================
        public event Action? OnChange;

        public void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        // =========================
        // LOAD DASHBOARD (PROFESSIONAL VERSION)
        // =========================
        public async Task LoadDashboardAsync(bool isAdmin = false)
        {
            // 1. Set the environment mode
            IsAdminEnvironment = isAdmin;

            // 2. Load core data via the service (Service should populate AllTasks or MyTasks)
            await _dashboardService.LoadDashboardAsync(this);

            // 3. Load activities shared across views
            RecentActivities = await _activityService.GetActivitiesAsync();

            // 4. Load Admin-Specific Extensions
            if (IsAdminEnvironment)
            {
                await LoadAdminSpecificDataAsync();
            }

            // 5. Default Mock Data (If not already loaded)
            EnsureDefaultData();

            NotifyStateChanged();
        }

        private async Task LoadAdminSpecificDataAsync()
        {
            // In a real app, you would call your admin-only service methods here
            // e.g., AuditLogs = await _adminService.GetAuditLogsAsync();

            if (!AuditLogs.Any())
            {
                AuditLogs = new List<AuditLogDto>
                {
                    new AuditLogDto { Action = "User Login", Entity = "auth", User = "Admin", Timestamp = DateTime.Now.AddMinutes(-5) },
                    new AuditLogDto { Action = "Invoice Generated", Entity = "invoice", User = "System", Timestamp = DateTime.Now.AddHours(-1) }
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
    }
}
