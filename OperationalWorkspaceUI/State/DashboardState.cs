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

        // =========================
        // TASK SELECTION (FIXED)
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
        // ACTIVITY
        // =========================
        public List<ActivityDto> RecentActivities { get; set; } = new();

        // =========================
        // AUDIT LOGS
        // =========================
        public List<AuditLogDto> AuditLogs { get; set; } = new();

        // =========================
        // KNOWLEDGE BASE
        // =========================
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();

        // =========================
        // REPORTS (FIXED MISSING PART)
        // =========================
        public List<ReportDto> AvailableReports { get; set; } = new();

        // =========================
        // CLIENTS (FIXED MISSING PART)
        // =========================
        public List<ClientDto> TopClients { get; set; } = new();

        // =========================
        // EMAIL + DASHBOARD
        // =========================
        public EmailInsightDto EmailContext { get; set; } = new();
        public DashboardDto DashboardData { get; set; } = new();

        public bool IsAdminEnvironment { get; private set; }

        // =========================
        // CRM / ERP / FINANCE
        // =========================
        public AdminErpDto AdminErp { get; set; } = new();
        public AdminCrmDto AdminCrm { get; set; } = new();
        public AdminFinanceDto AdminFinance { get; set; } = new();
        public AdminSystemHealthDto AdminHealth { get; set; } = new();

        public EmployeeErpDto EmployeeErp { get; set; } = new();
        public EmployeeCRMDTO EmployeeCrm { get; set; } = new();
        public EmployeeFinanceDto EmployeeFinance { get; set; } = new();

        // =========================
        // HELPERS
        // =========================
        public List<AuditLogDto> CurrentAuditLogs => AuditLogs;
        public List<ActivityDto> CurrentActivities => RecentActivities;

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
        // LOAD DASHBOARD
        // =========================
        public async Task LoadDashboardAsync()
        {
            await _dashboardService.LoadDashboardAsync(this);

            RecentActivities = await _activityService.GetActivitiesAsync();

            if (!AuditLogs.Any())
            {
                AuditLogs = new List<AuditLogDto>
                {
                    new AuditLogDto { Action = "User Login", Entity = "auth", User = "Admin", Timestamp = DateTime.Now.AddMinutes(-5) },
                    new AuditLogDto { Action = "Invoice Generated", Entity = "invoice", User = "System", Timestamp = DateTime.Now.AddHours(-1) }
                };
            }

            if (!KnowledgeBase.Any())
            {
                KnowledgeBase = new List<KnowledgeDto>
                {
                    new KnowledgeDto(Guid.NewGuid(), "Pricing Guide", "Content", "Sales", "Summary", "#"),
                    new KnowledgeDto(Guid.NewGuid(), "Shipping FAQ", "Content", "Support", "Summary", "#")
                };
            }

            NotifyStateChanged();
        }
    }
}