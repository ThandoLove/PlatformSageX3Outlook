using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using Task = System.Threading.Tasks.Task;

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
    // --- IActivityService ---
    // Assuming ActivityDto has a constructor based on your previous code
    public async Task<ActivityDto?> GetActivityByIdAsync(Guid id) =>
        new ActivityDto(id, "Meeting", "Sync", "Meeting", Guid.Empty, DateTime.UtcNow, "user@test.com", DateTime.UtcNow, "System");

    public async Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail) =>
        new ActivityDto(Guid.NewGuid(), dto.Title, dto.Description, dto.ActivityType, dto.RelatedEntityId ?? Guid.Empty, DateTime.UtcNow, userEmail, DateTime.UtcNow, "System");

    public async Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId) =>
        new List<ActivityDto> { new ActivityDto(Guid.NewGuid(), "Mock", "Desc", "Email", partnerId, DateTime.UtcNow, "user@test.com", DateTime.UtcNow, "System") };

    // --- IInventoryService ---
    public async Task<int> CountStockAlertsAsync() => 8;
    public async Task<InventoryItemDto?> GetItemAsync(Guid id, CancellationToken ct) => new InventoryItemDto();
    public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(string wh, CancellationToken ct) => new List<InventoryItemDto>();
    public async Task<StockAvailabilityResponse> CheckAvailabilityAsync(CheckStockRequest r, CancellationToken ct) => new StockAvailabilityResponse(true, 10);
    public async Task<AdjustStockResponse> AdjustStockAsync(StockAdjustmentRequest r, CancellationToken ct) => new AdjustStockResponse(true, "Success");
    public async Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(StockAdjustmentRequest r, CancellationToken ct) => new StockAdjustmentDto();

    // --- IBusinessPartnerService ---
    public async Task<string?> GetTopCustomerAsync(string userId) => "Global Industries Ltd";
    public async Task<string> GetRecentInteractionAsync(string userId) => "Last interaction: Yesterday via Email";
    public async Task<int> CountOpenOpportunitiesAsync() => 15;
    public async Task<int> CountOpenOpportunitiesAsync(string userId) => 4;
    public async Task<int> CountNewLeadsTodayAsync() => 3;
    public async Task<int> CountActiveCustomersAsync() => 450;

    public async Task<BusinessPartnersResponse?> GetSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct)
    {
        var snapshot = new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 500m, 5000m, 2, 1, 15000m, DateTime.UtcNow);
        return new BusinessPartnersResponse(snapshot);
    }
    public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(UpdateCreditLimitRequest req, CancellationToken ct) => new UpdateCreditLimitResponse(true);

    // --- ITaskService ---
    public async Task<TaskResponse> CreateAsync(CreateTaskRequest req, CancellationToken ct) => new TaskResponse { Id = Guid.NewGuid() };
    public async Task<CompleteTaskResponse> CompleteAsync(CompleteTaskRequest req, CancellationToken ct) => new CompleteTaskResponse(true, "Mock Done");
    public async Task<TaskListResponse> GetAsync(GetTasksRequest req, CancellationToken ct) => new TaskListResponse(new List<TaskDto>());
    public async Task<List<TaskDto>> GetTasksAssignedToAsync(string userId) => new List<TaskDto>();
    public async Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId) => new List<ApprovalDto>();
    public async Task<List<TaskDto>> GetAllTasksAsync() => new List<TaskDto>();
    public async Task<List<ApprovalDto>> GetAllPendingApprovalsAsync() => new List<ApprovalDto>();

    // --- ISalesService ---
    public async Task<int> CountOpenOrdersAsync(string userId) => 0;
    public async Task<int> CountPendingDeliveriesAsync(string userId) => 0;
    public async Task<int> CountTotalOrdersAsync() => 0;
    public async Task<CreateSalesOrderResponse> CreateOrderAsync(CreateSalesOrderRequest req, CancellationToken ct) => new CreateSalesOrderResponse(Guid.NewGuid());

    // FIXED: Using object initializer for SalesOrderDto
    public async Task<SalesOrderDetailsResponse?> GetOrderAsync(GetSalesOrderRequest req, CancellationToken ct) =>
        new SalesOrderDetailsResponse(new SalesOrderDto
        {
            Id = Guid.NewGuid(),
            OrderNumber = "MOCK-101",
            TotalAmount = 1500m,
            OrderDate = DateTime.UtcNow,
            OrderStatus = "Open"
        });

    // --- IKnowledgeService ---
    private async Task<KnowledgeDto?> GetKnowledgeByIdInternalAsync(Guid id) =>
        new KnowledgeDto(id, "Title", "Body", "Cat", "Summary", "url");

    public async Task<IEnumerable<KnowledgeDto>> SearchAsync(string query) => new List<KnowledgeDto>();
    public async Task<List<KnowledgeDto>> GetRecentArticlesAsync() => new List<KnowledgeDto>();

    // --- IEmailService ---
    public async Task<bool> SyncEmailAsync(EmailInsightDto dto) => true;

    // --- IInvoiceService Implementation ---
    public async Task<decimal> GetTotalOutstandingReceivablesAsync() => 250000.00m;
    public async Task<decimal> GetTotalMonthlySalesAsync() => 150000.00m;
    private async Task<InvoiceDto?> GetInvoiceByIdInternalAsync(Guid id) => new InvoiceDto { Id = id };
    public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize) => new List<InvoiceDto>();
    public async Task<InvoiceDto> CreateFromOrderAsync(Guid orderId) => new InvoiceDto { Id = Guid.NewGuid() };
    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto) { dto.Id = Guid.NewGuid(); return dto; }
    public async Task<decimal> GetOutstandingReceivablesAsync() => 50000.00m;
    public async Task<decimal> GetOutstandingReceivablesAsync(string userId) => 12000.00m;
    public async Task<decimal> GetMonthlySalesAsync(string userId) => 8500.00m;
    public async Task<int> CountOverdueInvoicesAsync() => 5;
    public async Task<int> CountInvoicesGeneratedAsync() => 120;
    public async Task<int> CountInvoicesDueAsync(string userId) => 3;
    public async Task<int> CountHighRiskAccountsAsync() => 2;

    // --- EXPLICIT INTERFACE IMPLEMENTATIONS ---
    Task<KnowledgeDto?> IKnowledgeService.GetByIdAsync(Guid id) => GetKnowledgeByIdInternalAsync(id);
    Task<InvoiceDto?> IInvoiceService.GetByIdAsync(Guid id) => GetInvoiceByIdInternalAsync(id);
    Task<ActivityDto?> IActivityService.GetByIdAsync(Guid id) => GetActivityByIdAsync(id);
}
