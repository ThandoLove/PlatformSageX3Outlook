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
        private readonly IInventoryService _inventoryService;
        private readonly ITaskService _taskService;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IAuditLogService _auditLogService;
        private readonly IIntegrationService _integrationService;
        private readonly IKnowledgeService _knowledgeService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserContextService _userContextService;

        // REMOVED: private string? topCustomerName; 
        // We define it locally inside the method now to avoid the "context" error.

        public DashboardService(
            ISalesService salesService,
            IInvoiceService invoiceService,
            IInventoryService inventoryService,
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
            _inventoryService = inventoryService;
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
                Finance = await BuildEmployeeFinanceAsync(userId),
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
                Finance = await BuildAdminFinanceAsync(),
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
                StockAlerts = await _inventoryService.CountStockAlertsAsync()
            };
        }

        private async Task<EmployeeCRMDTO> BuildEmployeeCRMAsync(string userId)
        {
            // FIX: This creates the 'topCustomerName' variable so it EXISTS in the context.
            // Since it returns a string, we use it directly.
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

        private async Task<EmployeeFinanceDto> BuildEmployeeFinanceAsync(string userId)
        {
            return new EmployeeFinanceDto
            {
                OutstandingReceivables = await _invoiceService.GetOutstandingReceivablesAsync(userId),
                ThisMonthSales = await _invoiceService.GetMonthlySalesAsync(userId)
            };
        }

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
                StockAlerts = await _inventoryService.CountStockAlertsAsync()
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

        private async Task<AdminFinanceDto> BuildAdminFinanceAsync()
        {
            return new AdminFinanceDto
            {
                TotalOutstandingReceivables = await _invoiceService.GetTotalOutstandingReceivablesAsync(),
                TotalSalesThisMonth = await _invoiceService.GetTotalMonthlySalesAsync()
            };
        }
    }
}
