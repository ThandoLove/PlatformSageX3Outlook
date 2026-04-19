using OperationalWorkspaceUI.UIServices.DashboardUI;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.Models.Email;
using OperationalWorkspaceApplication.Interfaces.IServices;
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

        public DashboardState(DashboardUIService dashboardService, IBusinessPartnerService bpService)
        {
            _dashboardService = dashboardService;
            _bpService = bpService;
        }

        // --- CONTACT LOGIC ---
        public int LateShipmentsCount { get; set; } = 48;
        public int OverdueTasksCount { get; set; } = 5;

        public async Task<bool> CheckContactExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var partner = await _bpService.GetPartnerByEmailAsync(email);
                return partner != null && partner.IsLinkedToSage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sage Lookup Error]: {ex.Message}");
                return false;
            }
        }

        // Environment flag
        public bool IsAdminEnvironment { get; private set; }

        // --- TICKETS (Replacing Opportunities) ---
        public List<TicketDto> AllTickets { get; set; } = new();
        public List<TicketDto> MyTickets { get; set; } = new();

        // --- ADMIN DATA ---
        public AdminErpDto AdminErp { get; set; } = new();
        public AdminCrmDto AdminCrm { get; set; } = new();
        public AdminFinanceDto AdminFinance { get; set; } = new();
        public AdminSystemHealthDto AdminHealth { get; set; } = new();
        public List<AuditLogDto> AuditLogs { get; set; } = new();

        // --- EMPLOYEE DATA ---
        public EmployeeErpDto EmployeeErp { get; set; } = new();
        public EmployeeCRMDTO EmployeeCrm { get; set; } = new();
        public EmployeeFinanceDto EmployeeFinance { get; set; } = new();
        public List<TaskDto> MyTasks { get; set; } = new();

        // --- SHARED DATA ---
        public List<ClientDto> TopClients { get; set; } = new();
        public List<ActivityDto> RecentActivities { get; set; } = new();
        public List<TaskDto> AllTasks { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public EmailContextDTO EmailContext { get; set; } = new();

        // --- CURRENT ENVIRONMENT DATA (Bound to UI) ---
        public AdminErpDto? CurrentErp => IsAdminEnvironment ? AdminErp : null;

        // Replaced Opportunities logic with Tickets in the CRM return
        public object CurrentCrm => IsAdminEnvironment ? AdminCrm : EmployeeCrm;

        public object CurrentFinance => IsAdminEnvironment ? AdminFinance : EmployeeFinance;
        public AdminSystemHealthDto? CurrentHealth => IsAdminEnvironment ? AdminHealth : null;
        public List<TaskDto> CurrentTasks => IsAdminEnvironment ? AllTasks : MyTasks;
        public List<TicketDto> CurrentTickets => IsAdminEnvironment ? AllTickets : MyTickets;
        public List<ActivityDto> CurrentActivities => RecentActivities;
        public List<AuditLogDto> CurrentAuditLogs => AuditLogs;

        // --- State change event ---
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        // --- Methods ---

        public void SetEnvironment(string environment)
        {
            IsAdminEnvironment = environment.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            NotifyStateChanged();
        }

        public async Task LoadDashboardAsync()
        {
            await _dashboardService.LoadDashboardAsync(this);

            // Filter Tasks and Tickets for the current user session
            MyTasks = AllTasks.Where(t => t.AssignedTo == "CurrentUser" && t.Status != "Completed").ToList();
            MyTickets = AllTickets.Where(t => t.AssignedTo == "CurrentUser").ToList();

            NotifyStateChanged();
        }

        public async Task LoadAdminDashboardAsync()
        {
            IsAdminEnvironment = true;
            await _dashboardService.LoadDashboardAsync(this);
            NotifyStateChanged();
        }

        public async Task LoadEmployeeDashboardAsync()
        {
            IsAdminEnvironment = false;
            await _dashboardService.LoadDashboardAsync(this);
            MyTasks = AllTasks.Where(t => t.AssignedTo == "CurrentUser" && t.Status != "Completed").ToList();
            MyTickets = AllTickets.Where(t => t.AssignedTo == "CurrentUser").ToList();
            NotifyStateChanged();
        }
    }
}
