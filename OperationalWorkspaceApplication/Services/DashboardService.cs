using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ISalesService _salesService;
        private readonly IInvoiceService _invoiceService;
        
        private readonly ITaskService _taskService;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IAuditLogService _auditLogService;
        private readonly IIntegrationService _integrationService;
        private readonly IKnowledgeService _knowledgeService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserContextService _userContextService;

        public DashboardService(
            ISalesService salesService,
            IInvoiceService invoiceService,
           
            ITaskService taskService,
            IBusinessPartnerService businessPartnerService,
            IAuditLogService auditLogService,
            IIntegrationService integrationService,
            IKnowledgeService knowledgeService,
            IAttachmentService attachmentService,
            IUserContextService userContextService
        )
        {
            _salesService = salesService;
            _invoiceService = invoiceService;
            
            _taskService = taskService;
            _businessPartnerService = businessPartnerService;
            _auditLogService = auditLogService;
            _integrationService = integrationService;
            _knowledgeService = knowledgeService;
            _attachmentService = attachmentService;
            _userContextService = userContextService;
        }

        public async Task<EmployeeDashboardDTO> GetEmployeeDashboardAsync(string userId)
        {
            var user = await _userContextService.GetUserAsync(userId);

            return new EmployeeDashboardDTO
            {
                UserName = user.Name,
                Role = user.Role,
                Connected = _integrationService.IsConnected(),
                ERP = await BuildEmployeeERPAsync(userId),
                CRM = await BuildEmployeeCRMAsync(userId),
                Tasks = await BuildEmployeeWorkAsync(userId),
                Approvals = await BuildEmployeeApprovalsAsync(userId),
                RecentActivity = await BuildEmployeeRecentActivityAsync(userId),
                // 🚀 FIXED: Cleared the assignment completely. This breaks the link to EmployeeFinanceDto!
                KnowledgeBase = await _knowledgeService.GetRecentArticlesAsync(),
                Attachments = await _attachmentService.GetRecentAttachmentsAsync(userId)
            };
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var user = await _userContextService.GetCurrentUserAsync();

            return new AdminDashboardDto
            {
                UserName = user.Name,
                Role = user.Role,
                Connected = _integrationService.IsConnected(),
                SystemHealth = await BuildAdminSystemHealthAsync(),
                ERP = await BuildAdminERPAsync(),
                CRM = await BuildAdminCRMAsync(),
                Tasks = await BuildAdminTasksAsync(),
                Approvals = await BuildAdminApprovalsAsync(),
                AuditLogs = await BuildAdminAuditAsync(),
                // 🚀 FIXED: Cleared the assignment completely. This breaks the link to AdminFinanceDto!
                KnowledgeBase = await _knowledgeService.GetRecentArticlesAsync(),
                Attachments = await _attachmentService.GetRecentAttachmentsAsync(user.Id)
            };
        }

        private async Task<EmployeeErpDto> BuildEmployeeERPAsync(string userId)
        {
            return new EmployeeErpDto
            {
                MyOpenOrders = await _salesService.CountOpenOrdersAsync(userId),
                InvoicesDue = await _invoiceService.CountInvoicesDueAsync(userId),
                PendingDeliveries = await _salesService.CountPendingDeliveriesAsync(userId),
                
            };
        }
        private async Task<EmployeeCRMDTO> BuildEmployeeCRMAsync(string userId)
        {
            string? topCustomerName = await _businessPartnerService.GetTopCustomerAsync(userId);

            return new EmployeeCRMDTO
            {
                TopCustomer = topCustomerName ?? "N/A",
                TopCustomerValue = 0,
                RecentInteraction = await _businessPartnerService.GetRecentInteractionAsync(userId),
                OpenOpportunities = await _businessPartnerService.CountOpenOpportunitiesAsync(userId)
            };
        }

        private async Task<List<TaskDto>> BuildEmployeeWorkAsync(string userId) =>
            await _taskService.GetTasksAssignedToAsync(userId);

        private async Task<List<ApprovalDto>> BuildEmployeeApprovalsAsync(string userId) =>
            await _taskService.GetPendingApprovalsAsync(userId);

        private async Task<List<ActivityDto>> BuildEmployeeRecentActivityAsync(string userId) =>
            await _auditLogService.GetRecentActivityForUserAsync(userId);

        private async Task<AdminSystemHealthDto> BuildAdminSystemHealthAsync()
        {
            return new AdminSystemHealthDto
            {
                SageX3Connected = _integrationService.IsConnected(),
                APIHealthStatus = await _integrationService.GetApiHealthStatusAsync(),
                FailedTransactions = await _integrationService.GetFailedTransactionsCountAsync(),
                PendingSyncJobs = await _integrationService.GetPendingSyncJobsCountAsync()
            };
        }

        private async Task<AdminErpDto> BuildAdminERPAsync()
        {
            return new AdminErpDto
            {
                TotalOrdersToday = await _salesService.CountTotalOrdersAsync(),
                InvoicesGenerated = await _invoiceService.CountInvoicesGeneratedAsync(),
                OverdueInvoices = await _invoiceService.CountOverdueInvoicesAsync(),
                
            };
        }

        private async Task<AdminCrmDto> BuildAdminCRMAsync()
        {
            return new AdminCrmDto
            {
                ActiveCustomers = await _businessPartnerService.CountActiveCustomersAsync(),
                NewLeadsToday = await _businessPartnerService.CountNewLeadsTodayAsync(),
                OpenOpportunities = await _businessPartnerService.CountOpenOpportunitiesAsync()
            };
        }

        private async Task<List<TaskDto>> BuildAdminTasksAsync() =>
            await _taskService.GetAllTasksAsync();

        private async Task<List<ApprovalDto>> BuildAdminApprovalsAsync() =>
            await _taskService.GetAllPendingApprovalsAsync();

        private async Task<List<AuditLogDto>> BuildAdminAuditAsync() =>
            await _auditLogService.GetAllRecentLogsAsync();
    }
}
