using OperationalWorkspaceUI.UIServices.DashboardUI;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.Models.Email;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.State
{
    public class DashboardState
    {
        private readonly DashboardUIService _dashboardService;

        public DashboardState(DashboardUIService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // Environment flag
        public bool IsAdminEnvironment { get; private set; }

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

        // --- CURRENT ENVIRONMENT DATA (read-only for UI binding) ---
        public AdminErpDto? CurrentErp => IsAdminEnvironment ? AdminErp : null;
        public object CurrentCrm => IsAdminEnvironment ? AdminCrm : EmployeeCrm;
        public object CurrentFinance => IsAdminEnvironment ? AdminFinance : EmployeeFinance;
        public AdminSystemHealthDto? CurrentHealth => IsAdminEnvironment ? AdminHealth : null;
        public List<TaskDto> CurrentTasks => IsAdminEnvironment ? AllTasks : MyTasks;
        public List<ActivityDto> CurrentActivities => RecentActivities;
        public List<AuditLogDto> CurrentAuditLogs => AuditLogs;

        // --- State change event ---
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        // --- Methods ---

        /// <summary>
        /// Sets environment based on Sage X3 info (Admin/Employee)
        /// </summary>
        public void SetEnvironment(string environment)
        {
            IsAdminEnvironment = environment.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            NotifyStateChanged();
        }

        /// <summary>
        /// Loads all data for both Admin and Employee dashboards
        /// </summary>
        public async Task LoadDashboardAsync()
        {
            await _dashboardService.LoadDashboardAsync(this);

            // Example: filter employee tasks
            MyTasks = AllTasks.Where(t => t.AssignedTo == "CurrentUser" && t.Status != "Completed").ToList();

            NotifyStateChanged();
        }

        /// <summary>
        /// Loads admin-specific dashboard data
        /// </summary>
        public async Task LoadAdminDashboardAsync()
        {
            IsAdminEnvironment = true;
            await _dashboardService.LoadDashboardAsync(this);
            NotifyStateChanged();
        }

        /// <summary>
        /// Loads employee-specific dashboard data
        /// </summary>
        public async Task LoadEmployeeDashboardAsync()
        {
            IsAdminEnvironment = false;
            await _dashboardService.LoadDashboardAsync(this);

            // Example: filter employee tasks
            MyTasks = AllTasks.Where(t => t.AssignedTo == "CurrentUser" && t.Status != "Completed").ToList();

            NotifyStateChanged();
        }
    }
}