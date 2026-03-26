
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
    
  

    public class DashboardState
    {
        public AdminErpDto ERPData { get; set; }
        public AdminCrmDto CRMData { get; set; }
        public List<TaskDto> Tasks { get; set; } = new();
        public AdminFinanceDto FinanceData { get; set; }
        public AdminSystemHealthDto SystemHealth { get; set; }
        public List<ActivityDto> RecentActivity { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<AuditLogDto> AuditLogs { get; set; } = new();
    }
}
