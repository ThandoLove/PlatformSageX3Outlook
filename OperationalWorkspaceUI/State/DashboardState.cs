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

        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedEmployeeFilter { get; set; } = string.Empty;
        public string SelectedDepartmentFilter { get; set; } = string.Empty;

        public bool IsSidebarCollapsed { get; set; } = false;

        public void ToggleGlobalSidebar()
        {
            IsSidebarCollapsed = !IsSidebarCollapsed;
            NotifyStateChanged();
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

        public bool ShowAddClientUI { get; set; } = false;
        public EmailContextDto? ActiveCompositeContext { get; set; }

        public void SetEmailContextState(EmailContextDto? context, bool isUnknownSender)
        {
            if (context == null)
                return;

            ActiveCompositeContext = context;
            ShowAddClientUI = isUnknownSender;

            if (context.Email != null)
            {
                EmailContext = context.Email;
            }

            NotifyStateChanged();
        }

        public DashboardDto DashboardData { get; set; } = new();
        public AdminErpDto AdminErp { get; set; } = new();
        public AdminCrmDto AdminCrm { get; set; } = new();

        // 🚀 CRITICAL DECOMMISSIONING: AdminFinanceDto property line has been completely removed!
        public AdminSystemHealthDto AdminHealth { get; set; } = new AdminSystemHealthDto();
        public EmployeeErpDto EmployeeErp { get; set; } = new EmployeeErpDto();
        public EmployeeCRMDTO EmployeeCrm { get; set; } = new EmployeeCRMDTO();

        // 🚀 CRITICAL DECOMMISSIONING: EmployeeFinanceDto property line has been completely removed!

        public object CurrentCrm =>
            IsAdminEnvironment
            ? (object)AdminCrm
            : EmployeeCrm;

        // 🚀 CRITICAL DECOMMISSIONING: CurrentFinance object helper method completely removed to wipe out type references!
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

                EnsureDefaultData();
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DashboardState Load Error: {ex.Message}");
            }
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
