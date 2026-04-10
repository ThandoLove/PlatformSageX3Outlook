using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OperationalWorkspaceApplication.Services;

public sealed class MockUnifiedService :
    IActivityService,
    IEmailService,
    IKnowledgeService,
    ISalesService,
    IBusinessPartnerService,
    IInventoryService,
    ITaskService,
    IInvoiceService
{
    // --- IActivityService Implementation ---
    async System.Threading.Tasks.Task<ActivityDto?> IActivityService.GetByIdAsync(Guid id) =>
        new ActivityDto(id, "Meeting", "Sync", "Meeting", Guid.Empty, DateTime.UtcNow, "user@test.com", DateTime.UtcNow, "System");

    public async System.Threading.Tasks.Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail) =>
        new ActivityDto(Guid.NewGuid(), dto.Title, dto.Description, dto.ActivityType, dto.RelatedEntityId ?? Guid.Empty, DateTime.UtcNow, userEmail, DateTime.UtcNow, "System");

    public async System.Threading.Tasks.Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId) =>
        new List<ActivityDto> { new ActivityDto(Guid.NewGuid(), "Mock", "Desc", "Email", partnerId, DateTime.UtcNow, "user@test.com", DateTime.UtcNow, "System") };

    public async System.Threading.Tasks.Task AttachEmailAsync(object model) => await System.Threading.Tasks.Task.CompletedTask;
    public async System.Threading.Tasks.Task LogAsync(ActivityDto activity) => await System.Threading.Tasks.Task.CompletedTask;

    // --- IKnowledgeService Implementation ---
    async System.Threading.Tasks.Task<KnowledgeDto?> IKnowledgeService.GetByIdAsync(Guid id) =>
        new KnowledgeDto(id, "Title", "Body", "Cat", "Summary", "url");

    public async System.Threading.Tasks.Task<IEnumerable<KnowledgeDto>> SearchAsync(string query) => new List<KnowledgeDto>();
    public async System.Threading.Tasks.Task<List<KnowledgeDto>> GetRecentArticlesAsync() => new List<KnowledgeDto>();
    public async System.Threading.Tasks.Task SendKnowledgeAsync(object model) => await System.Threading.Tasks.Task.CompletedTask;

    // --- IInvoiceService Implementation ---
    async System.Threading.Tasks.Task<InvoiceDto?> IInvoiceService.GetByIdAsync(Guid id) => new InvoiceDto { Id = id };

    public async System.Threading.Tasks.Task<decimal> GetTotalOutstandingReceivablesAsync() => 250000.00m;
    public async System.Threading.Tasks.Task<decimal> GetTotalMonthlySalesAsync() => 150000.00m;
    public async System.Threading.Tasks.Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize) => new List<InvoiceDto>();
    public async System.Threading.Tasks.Task<InvoiceDto> CreateFromOrderAsync(Guid orderId) => new InvoiceDto { Id = Guid.NewGuid() };
    public async System.Threading.Tasks.Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto) { dto.Id = Guid.NewGuid(); return dto; }
    public async System.Threading.Tasks.Task<decimal> GetOutstandingReceivablesAsync() => 50000.00m;
    public async System.Threading.Tasks.Task<decimal> GetOutstandingReceivablesAsync(string userId) => 12000.00m;
    public async System.Threading.Tasks.Task<decimal> GetMonthlySalesAsync(string userId) => 8500.00m;
    public async System.Threading.Tasks.Task<int> CountOverdueInvoicesAsync() => 5;
    public async System.Threading.Tasks.Task<int> CountInvoicesGeneratedAsync() => 120;
    public async System.Threading.Tasks.Task<int> CountInvoicesDueAsync(string userId) => 3;
    public async System.Threading.Tasks.Task<int> CountHighRiskAccountsAsync() => 2;

    // --- IEmailService ---
    // --- IEmailService ---
    public async System.Threading.Tasks.Task<bool> SyncEmailAsync(EmailInsightDto dto) => true;

    // ADDED: Implementation for GetEmailByIdAsync
    public async System.Threading.Tasks.Task<EmailInsightDto?> GetEmailByIdAsync(string emailId) =>
        new EmailInsightDto { MessageId = emailId, Subject = "Mock Subject", From = "test@example.com" };

    // ADDED: Implementation for GetLinkedOrdersAsync
    public async System.Threading.Tasks.Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId) =>
        new List<OpenOrderDto>();

    // ADDED: Implementation for GetLinkedTasksAsync
    public async System.Threading.Tasks.Task<List<TaskDto>> GetLinkedTasksAsync(string emailId) =>
        new List<TaskDto>();


    // --- IInventoryService ---
    public async System.Threading.Tasks.Task<int> CountStockAlertsAsync() => 8;
    public async System.Threading.Tasks.Task<InventoryItemDto?> GetItemAsync(Guid id, CancellationToken ct) => new InventoryItemDto();
    public async System.Threading.Tasks.Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(string wh, CancellationToken ct) => new List<InventoryItemDto>();
    public async System.Threading.Tasks.Task<StockAvailabilityResponse> CheckAvailabilityAsync(CheckStockRequest r, CancellationToken ct) => new StockAvailabilityResponse(true, 10);
    public async System.Threading.Tasks.Task<AdjustStockResponse> AdjustStockAsync(StockAdjustmentRequest r, CancellationToken ct) => new AdjustStockResponse(true, "Success");
    public async System.Threading.Tasks.Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(StockAdjustmentRequest r, CancellationToken ct) => new StockAdjustmentDto();

    // --- IBusinessPartnerService ---
    public async System.Threading.Tasks.Task<string?> GetTopCustomerAsync(string userId) => "Global Industries Ltd";
    public async System.Threading.Tasks.Task<string> GetRecentInteractionAsync(string userId) => "Last interaction: Yesterday via Email";
    public async System.Threading.Tasks.Task<int> CountOpenOpportunitiesAsync() => 15;
    public async System.Threading.Tasks.Task<int> CountOpenOpportunitiesAsync(string userId) => 4;
    public async System.Threading.Tasks.Task<int> CountNewLeadsTodayAsync() => 3;
    public async System.Threading.Tasks.Task<int> CountActiveCustomersAsync() => 450;
    public async System.Threading.Tasks.Task<BusinessPartnersResponse?> GetSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct) =>
        new BusinessPartnersResponse(new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 500m, 5000m, 2, 1, 15000m, DateTime.UtcNow));
    public async System.Threading.Tasks.Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(UpdateCreditLimitRequest req, CancellationToken ct) => new UpdateCreditLimitResponse(true);

    // Implementation for GetPartnerByEmailAsync required by IBusinessPartnerService
    public async System.Threading.Tasks.Task<BusinessPartnerSnapshotDto?> GetPartnerByEmailAsync(string? email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return await System.Threading.Tasks.Task.FromResult<BusinessPartnerSnapshotDto?>(null);

        var normalized = email!.Trim();

        if (normalized.Equals("sarah.johnson@techinnovate.com", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("test@example.com", StringComparison.OrdinalIgnoreCase))
        {
            var dto = new BusinessPartnerSnapshotDto("C1000", "Tech Innovations Inc.", 10000m, 2500m, 3, 1, 1500m, DateTime.UtcNow)
            {
                FullName = "Sarah Johnson",
                IsLinkedToSage = true,
                Location = "Los Angeles, CA",
                AssignedRep = "John Smith",
                Timeline = new List<ActivityDto>
                {
                    new ActivityDto { Title = "Email", Action = "Quote Request", Timestamp = DateTime.UtcNow.AddDays(-2) },
                    new ActivityDto { Title = "Call", Action = "Follow-up", Timestamp = DateTime.UtcNow.AddDays(-5) }
                }
            };

            return await System.Threading.Tasks.Task.FromResult(dto);
        }

        return await System.Threading.Tasks.Task.FromResult<BusinessPartnerSnapshotDto?>(null);
    }

    // --- ITaskService ---
    public async System.Threading.Tasks.Task<TaskResponse> CreateAsync(CreateTaskRequest req, CancellationToken ct) => new TaskResponse { Id = Guid.NewGuid() };
    public async System.Threading.Tasks.Task<CompleteTaskResponse> CompleteAsync(CompleteTaskRequest req, CancellationToken ct) => new CompleteTaskResponse(true, "Mock Done");
    public async System.Threading.Tasks.Task<TaskListResponse> GetAsync(GetTasksRequest req, CancellationToken ct) => new TaskListResponse(new List<TaskDto>());
    public async System.Threading.Tasks.Task<List<TaskDto>> GetTasksAssignedToAsync(string userId) => new List<TaskDto>();
    public async System.Threading.Tasks.Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId) => new List<ApprovalDto>();
    public async System.Threading.Tasks.Task<List<TaskDto>> GetAllTasksAsync() => new List<TaskDto>();
    public async System.Threading.Tasks.Task<List<ApprovalDto>> GetAllPendingApprovalsAsync() => new List<ApprovalDto>();

    // --- ISalesService ---
    public async System.Threading.Tasks.Task<int> CountOpenOrdersAsync(string userId) => 0;
    public async System.Threading.Tasks.Task<int> CountPendingDeliveriesAsync(string userId) => 0;
    public async System.Threading.Tasks.Task<int> CountTotalOrdersAsync() => 0;
    public async System.Threading.Tasks.Task<CreateSalesOrderResponse> CreateOrderAsync(CreateSalesOrderRequest req, CancellationToken ct) => new CreateSalesOrderResponse(Guid.NewGuid());
    public async System.Threading.Tasks.Task<SalesOrderDetailsResponse?> GetOrderAsync(GetSalesOrderRequest req, CancellationToken ct) =>
        new SalesOrderDetailsResponse(new SalesOrderDto { Id = Guid.NewGuid(), OrderNumber = "MOCK-101", TotalAmount = 1500m, OrderDate = DateTime.UtcNow, OrderStatus = "Open" });
    // ADD THIS INSIDE MockUnifiedService

    public async System.Threading.Tasks.Task<CreateClientFromEmailResponse> CreateFromEmailAsync(
        CreateClientFromEmailRequest request,
        CancellationToken ct = default)
    {
        // Minimal mock logic (DO NOT OVERTHINK)

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");

        return await System.Threading.Tasks.Task.FromResult(
            new CreateClientFromEmailResponse
            {
                Id = Guid.NewGuid(),
                Code = $"MOCK-{request.Email}"
            });
    }


}
