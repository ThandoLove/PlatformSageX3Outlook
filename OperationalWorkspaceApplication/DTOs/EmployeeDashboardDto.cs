using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
    // CODE START
    public class EmployeeDashboardDTO
    {
        // Basic user info
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool Connected { get; set; }

        // ERP Section (Personal)
        public EmployeeErpDto ERP { get; set; } = new EmployeeErpDto();

        // CRM Section (Light)
        public EmployeeCRMDTO CRM { get; set; } = new EmployeeCRMDTO();

        // Tasks assigned
        public List<TaskDto> Tasks { get; set; }= new List<TaskDto>();

        // Approvals (if allowed)
        public List<ApprovalDto> Approvals { get; set; } = new List<ApprovalDto>();

        // Recent Activity
        public List<ActivityDto> RecentActivity { get; set; } = new List<ActivityDto>();

        // Financial snapshot
        public EmployeeFinanceDto Finance { get; set; } = new EmployeeFinanceDto();

        // Knowledge Base (optional)
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new List<KnowledgeDto>();

        // Attachments (optional)
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
    }
    // CODE END
}
