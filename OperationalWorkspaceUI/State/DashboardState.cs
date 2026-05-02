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

        // --- ORDER CONTEXT ---
        public string? SelectedOrderId { get; private set; }
        public async Task LoadOrderDetailsAsync(string? orderId)
        {
            SelectedOrderId = orderId;
            if (orderId != null)
            {
                EmailContext = new EmailInsightDto { SenderName = "Sage X3", Subject = $"Order: {orderId}", ReceivedAt = DateTime.Now };
            }
            NotifyStateChanged();
            await Task.CompletedTask;
        }

        // --- DATA LISTS ---
        public List<AuditLogDto> AuditLogs { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<TicketDto> AllTickets { get; set; } = new();
        public List<TicketDto> MyTickets { get; set; } = new();
        public List<ReportDto> AvailableReports { get; set; } = new();
        public List<ActivityDto> RecentActivities { get; set; } = new();
        public List<TaskDto> AllTasks { get; set; } = new();
        public List<TaskDto> MyTasks { get; set; } = new();
        public List<ClientDto> TopClients { get; set; } = new();

        public EmailInsightDto EmailContext { get; set; } = new();
        public DashboardDto DashboardData { get; set; } = new();
        public bool IsAdminEnvironment { get; private set; }

        // --- DTO OBJECTS ---
        public AdminErpDto AdminErp { get; set; } = new();
        public AdminCrmDto AdminCrm { get; set; } = new();
        public AdminFinanceDto AdminFinance { get; set; } = new();
        public AdminSystemHealthDto AdminHealth { get; set; } = new();
        public EmployeeErpDto EmployeeErp { get; set; } = new();
        public EmployeeCRMDTO EmployeeCrm { get; set; } = new();
        public EmployeeFinanceDto EmployeeFinance { get; set; } = new();

        // --- HELPER PROPERTIES (FIXES UI ERRORS) ---
        public List<AuditLogDto> CurrentAuditLogs => AuditLogs;
        public List<TicketDto> CurrentTickets => IsAdminEnvironment ? AllTickets : MyTickets;
        public List<ActivityDto> CurrentActivities => RecentActivities;
        public List<TaskDto> CurrentTasks => IsAdminEnvironment ? AllTasks : MyTasks;
        public object CurrentCrm => IsAdminEnvironment ? (object)AdminCrm : (object)EmployeeCrm;
        public object CurrentFinance => IsAdminEnvironment ? (object)AdminFinance : (object)EmployeeFinance;

        public event Action? OnChange;
        public void NotifyStateChanged() => OnChange?.Invoke();

        public async Task LoadDashboardAsync()
        {
            await _dashboardService.LoadDashboardAsync(this);
            RecentActivities = await _activityService.GetActivitiesAsync();

            // Seed Audit Logs if empty
            if (!AuditLogs.Any())
            {
                AuditLogs = new List<AuditLogDto>
                {
                    new AuditLogDto { Action = "User Login", Entity = "auth", User = "Admin", Timestamp = DateTime.Now.AddMinutes(-5) },
                    new AuditLogDto { Action = "Invoice Generated", Entity = "invoice", User = "System", Timestamp = DateTime.Now.AddHours(-1) }
                };
            }

            // Seed Knowledge Base (Positional Record Fix)
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
