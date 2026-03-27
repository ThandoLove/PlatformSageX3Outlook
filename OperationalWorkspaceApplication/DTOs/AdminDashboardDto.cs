using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
    // CODE START
    public class AdminDashboardDto
    {
        // Basic info
        public string UserName { get; set; } = "Admin User";
        public string Role { get; set; } = "Administrator";
        public bool Connected { get; set; }

        // System Health
        public AdminSystemHealthDto SystemHealth { get; set; } = new AdminSystemHealthDto();

        // ERP Overview (Global)
        public AdminErpDto ERP { get; set; } = new AdminErpDto();

        // CRM Overview (Light summary)
        public AdminCrmDto CRM { get; set; } = new AdminCrmDto();

        // Task Management (All tasks)
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();

        // Approvals (Global)
        public List<ApprovalDto> Approvals { get; set; } = new List<ApprovalDto>();

        // Audit & Activity
        public List<AuditLogDto> AuditLogs { get; set; } = new List<AuditLogDto>();

        // Financial Watch
        public AdminFinanceDto Finance { get; set; }= new AdminFinanceDto();

        // Knowledge Base (shared)
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new List<KnowledgeDto>();

        // Attachments (shared)
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
        public List<ActivityDto> RecentActivity { get; set; }
    }
    // CODE END
}
