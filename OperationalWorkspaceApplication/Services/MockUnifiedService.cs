using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


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
    // --- ISalesService Implementation (FIXED RETURN TYPES) ---
    public async System.Threading.Tasks.Task<CreateSalesOrderResponse> CreateOrderAsync(CreateSalesOrderRequest req, CancellationToken ct)
    {
        return await System.Threading.Tasks.Task.FromResult(new CreateSalesOrderResponse(Guid.NewGuid()));
    }

    public async System.Threading.Tasks.Task<SalesOrderDetailsResponse?> GetOrderAsync(GetSalesOrderRequest req, CancellationToken ct)
    {
        // FIX: Matching SalesOrderDto(id, orderNumber, totalAmount) constructor
        var mockDto = new SalesOrderDto(Guid.NewGuid(), "MOCK-101", 1500.00m);
        return await System.Threading.Tasks.Task.FromResult(new SalesOrderDetailsResponse(mockDto));
    }

    // --- IBusinessPartnerService Implementation (FIXED CONSTRUCTOR) ---
    public async System.Threading.Tasks.Task<BusinessPartnersResponse?> GetSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct)
    {
        // FIX: Matching BusinessPartnerSnapshotDto(BpCode, Name, Balance, CreditLimit, OpenOrders, OpenInvoices, TotalSales, LastSaleDate)
        var snapshot = new BusinessPartnerSnapshotDto(
            "C1000",             // BpCode
            "Mock Corporation",   // Name
            500.00m,             // Balance
            5000.00m,            // CreditLimit
            2,                   // OpenOrders
            1,                   // OpenInvoices
            15000.00m,           // TotalSales
            DateTime.UtcNow      // LastSaleDate
        );
        return await System.Threading.Tasks.Task.FromResult(new BusinessPartnersResponse(snapshot));
    }

    public async System.Threading.Tasks.Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(UpdateCreditLimitRequest req, CancellationToken ct)
    {
        return await System.Threading.Tasks.Task.FromResult(new UpdateCreditLimitResponse(true));
    }

    // --- IActivityService ---
    public async System.Threading.Tasks.Task<ActivityDto?> GetByIdAsync(Guid id) =>
        new ActivityDto(id, "Mock Meeting", "ERP Sync", "Meeting", null, DateTime.UtcNow, "test@erp.com");

    public async System.Threading.Tasks.Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail) =>
        new ActivityDto(Guid.NewGuid(), dto.Title, dto.Description, dto.ActivityType, dto.RelatedEntityId, DateTime.UtcNow, userEmail);

    public async System.Threading.Tasks.Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId) =>
        new List<ActivityDto> { new ActivityDto(Guid.NewGuid(), "Mock", "Desc", "Email", partnerId, DateTime.UtcNow, "test@erp.com") };

    // --- Other Mocks (Simplified for clarity) ---
    public async System.Threading.Tasks.Task<bool> SyncEmailAsync(EmailInsightDto dto) => true;

    public async System.Threading.Tasks.Task<KnowledgeDto?> GetKnowledgeByIdAsync(Guid id) =>
        new KnowledgeDto(id, "Title", "Content", "Cat");

    public async System.Threading.Tasks.Task<IEnumerable<KnowledgeDto>> SearchAsync(string query) => new List<KnowledgeDto>();

    public async System.Threading.Tasks.Task<InvoiceDto?> GetInvoiceByIdAsync(Guid id) => new InvoiceDto { Id = id };

    public async System.Threading.Tasks.Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize) => new List<InvoiceDto>();

    public async System.Threading.Tasks.Task<InvoiceDto> CreateFromOrderAsync(Guid orderId) => new InvoiceDto { Id = Guid.NewGuid() };

    public async System.Threading.Tasks.Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto) { dto.Id = Guid.NewGuid(); return dto; }

    public async System.Threading.Tasks.Task<TaskResponse> CreateAsync(CreateTaskRequest req, CancellationToken ct) => new TaskResponse { Id = Guid.NewGuid() };

    public async System.Threading.Tasks.Task<CompleteTaskResponse> CompleteAsync(CompleteTaskRequest req, CancellationToken ct) => new CompleteTaskResponse(true, "Done");

    public async System.Threading.Tasks.Task<TaskListResponse> GetAsync(GetTasksRequest req, CancellationToken ct) => new TaskListResponse(new List<TaskDto>());

    public async System.Threading.Tasks.Task<InventoryItemDto?> GetItemAsync(Guid id, CancellationToken ct) => new InventoryItemDto();

    public async System.Threading.Tasks.Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(string wh, CancellationToken ct) => new List<InventoryItemDto>();

    public async System.Threading.Tasks.Task<StockAvailabilityResponse> CheckAvailabilityAsync(CheckStockRequest r, CancellationToken ct) => new StockAvailabilityResponse(true, 10);

    public async System.Threading.Tasks.Task<AdjustStockResponse> AdjustStockAsync(StockAdjustmentRequest r, CancellationToken ct) => new AdjustStockResponse(true, "OK");

    public async System.Threading.Tasks.Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(StockAdjustmentRequest r, CancellationToken ct) => new StockAdjustmentDto();

    System.Threading.Tasks.Task<KnowledgeDto?> IKnowledgeService.GetByIdAsync(Guid id) => GetKnowledgeByIdAsync(id);
    System.Threading.Tasks.Task<InvoiceDto?> IInvoiceService.GetByIdAsync(Guid id) => GetInvoiceByIdAsync(id);
}
