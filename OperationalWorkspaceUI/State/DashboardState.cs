using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceUI.State
{
    public class DashboardState
    {
        private readonly IDashboardService _dashboardService;

        public DashboardState(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // =========================
        // RAW DATA (FROM APPLICATION)
        // =========================
        public AdminErpDto ERPData { get; private set; } = new();
        public AdminCrmDto CRMData { get; private set; } = new();
        public AdminFinanceDto FinanceData { get; private set; } = new();
        public AdminSystemHealthDto SystemHealth { get; private set; } = new();

        public List<TaskDto> Tasks { get; private set; } = new();
        public List<ActivityDto> RecentActivity { get; private set; } = new();
        public List<AuditLogDto> AuditLogs { get; private set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; private set; } = new();

        // =========================
        // UI-FRIENDLY PROPERTIES (🔥 IMPORTANT)
        // =========================

        // CRM Panel
        public List<ClientDTO> TopClients =>
            CRMData?.TopClients ?? new List<ClientDTO>();

        // Tasks Panel
        public List<TaskDto> AllTasks => Tasks;

        // Activity Panel
        public List<ActivityDto> RecentActivities => RecentActivity;

        // =========================
        // LOAD DATA (🔥 REQUIRED)
        // =========================
        public async Task LoadAdminDashboardAsync()
        {
            var data = await _dashboardService.GetAdminDashboardAsync();

            ERPData = data.ERP;
            CRMData = data.CRM;
            FinanceData = data.Finance;
            SystemHealth = data.SystemHealth;

            Tasks = data.Tasks;
            RecentActivity = data.RecentActivity;
            AuditLogs = data.AuditLogs;
            KnowledgeBase = data.KnowledgeBase;
        }
    }
}