// CODE START

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
    // ---------------- ACTIVITY ----------------
    async Task<ActivityDto?> IActivityService.GetByIdAsync(Guid id) =>
        new ActivityDto(id, "Meeting", "Sync", "Meeting", Guid.Empty, DateTime.UtcNow, "user@test.com", DateTime.UtcNow, "System");

    public async Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail) =>
        new ActivityDto(Guid.NewGuid(), dto.Title, dto.Description, dto.ActivityType, dto.RelatedEntityId ?? Guid.Empty, DateTime.UtcNow, userEmail, DateTime.UtcNow, "System");

    public async Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId) =>
        new List<ActivityDto> { new ActivityDto(Guid.NewGuid(), "Mock", "Desc", "Email", partnerId, DateTime.UtcNow, "user@test.com", DateTime.UtcNow, "System") };

    public async Task AttachEmailAsync(object model) => await Task.CompletedTask;
    public async Task LogAsync(ActivityDto activity) => await Task.CompletedTask;

    // ---------------- KNOWLEDGE ----------------
    async Task<KnowledgeDto?> IKnowledgeService.GetByIdAsync(Guid id) =>
        new KnowledgeDto(id, "Title", "Body", "Cat", "Summary", "url");

    public async Task<IEnumerable<KnowledgeDto>> SearchAsync(string query) => new List<KnowledgeDto>();
    public async Task<List<KnowledgeDto>> GetRecentArticlesAsync() => new List<KnowledgeDto>();
    public async Task SendKnowledgeAsync(object model) => await Task.CompletedTask;

    // ---------------- EMAIL ----------------
    public async Task<bool> SyncEmailAsync(EmailInsightDto dto) => true;

    public async Task<EmailInsightDto?> GetEmailByIdAsync(string emailId) =>
        new EmailInsightDto { MessageId = emailId, Subject = "Mock Subject", From = "test@example.com" };

    public async Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId) =>
        new List<OpenOrderDto>();

    public async Task<List<TaskDto>> GetLinkedTasksAsync(string emailId) =>
        new List<TaskDto>();

    // ---------------- INVENTORY ----------------
    public async Task<int> CountStockAlertsAsync() => 8;

    public async Task<InventoryItemDto?> GetItemAsync(Guid id, CancellationToken ct) =>
        new InventoryItemDto();

    public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(string wh, CancellationToken ct) =>
        new List<InventoryItemDto>();

    public async Task<StockAvailabilityResponse> CheckAvailabilityAsync(CheckStockRequest r, CancellationToken ct) =>
        new StockAvailabilityResponse(true, 10);

    public async Task<AdjustStockResponse> AdjustStockAsync(StockAdjustmentRequest r, CancellationToken ct) =>
        new AdjustStockResponse(true, "Success");

    public async Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(StockAdjustmentRequest r, CancellationToken ct) =>
        new StockAdjustmentDto();

    // ---------------- BUSINESS PARTNER ----------------
    public async Task<string?> GetTopCustomerAsync(string userId) => "Global Industries Ltd";

    public async Task<string> GetRecentInteractionAsync(string userId) =>
        "Last interaction: Yesterday via Email";

    public async Task<int> CountOpenOpportunitiesAsync() => 15;

    public async Task<int> CountOpenOpportunitiesAsync(string userId) => 4;

    public async Task<int> CountNewLeadsTodayAsync() => 3;

    public async Task<int> CountActiveCustomersAsync() => 450;

    public async Task<BusinessPartnersResponse?> GetSnapshotAsync(
        GetBusinessPartnerSnapshotRequest req,
        CancellationToken ct) =>
        new BusinessPartnersResponse(
            new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 500m, 5000m, 2, 1, 15000m, DateTime.UtcNow)
        );

    public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(
        UpdateCreditLimitRequest req,
        CancellationToken ct) =>
        new UpdateCreditLimitResponse(true);

    public async Task<BusinessPartnerSnapshotDto?> GetPartnerByEmailAsync(
        string? email,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        return new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 500m, 5000m, 2, 1, 15000m, DateTime.UtcNow)
        {
            FullName = "Mock User",
            IsLinkedToSage = true,
            Location = "Mock City",
            AssignedRep = "Mock Rep",
            Timeline = new List<ActivityDto>()
        };
    }

    public async Task<CreateClientFromEmailResponse> CreateFromEmailAsync(
        CreateClientFromEmailRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");

        return new CreateClientFromEmailResponse
        {
            Id = Guid.NewGuid(),
            Code = $"MOCK-{request.Email}"
        };
    }
        // Add this method inside the MockUnifiedService class
        public async Task<bool> CreateContactAsync(ContactCreateDto contact)
    {
        // This is a mock, so we just simulate a successful save
        await Task.Delay(500); // Simulate a tiny bit of network lag
        return true;
    }

    

    // ---------------- TASK ----------------
    public async Task<TaskResponse> CreateAsync(CreateTaskRequest req, CancellationToken ct) =>
        new TaskResponse { Id = Guid.NewGuid() };

    public async Task<CompleteTaskResponse> CompleteAsync(CompleteTaskRequest req, CancellationToken ct) =>
        new CompleteTaskResponse(true, "Done");

    public async Task<TaskListResponse> GetAsync(GetTasksRequest req, CancellationToken ct) =>
        new TaskListResponse(new List<TaskDto>());

    public async Task<List<TaskDto>> GetTasksAssignedToAsync(string userId) =>
        new List<TaskDto>();

    public async Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId) =>
        new List<ApprovalDto>();

    public async Task<List<TaskDto>> GetAllTasksAsync() =>
        new List<TaskDto>();

    public async Task<List<ApprovalDto>> GetAllPendingApprovalsAsync() =>
        new List<ApprovalDto>();


    public async Task<TaskResponse> DelegateAsync(DelegateTaskRequest request, CancellationToken ct)
    {
        // 1. Simulate a tiny delay for the mock
        await Task.Delay(50, ct);

        // 2. Find the task in your local mock list
        // This assumes you have a mock list of tasks in this service
        // If not, just returning a Success response is enough to fix the build
        return new TaskResponse
        {
            IsSuccess = true,
            Message = "Mock: Task delegated successfully",
            Id = request.TaskId
        };
    }
    // ---------------- SALES ----------------
    public async Task<int> CountOpenOrdersAsync(string userId) => 0;

    public async Task<int> CountPendingDeliveriesAsync(string userId) => 0;

    public async Task<int> CountTotalOrdersAsync() => 0;

    public async Task<CreateSalesOrderResponse> CreateOrderAsync(
        CreateSalesOrderRequest req,
        CancellationToken ct) =>
        new CreateSalesOrderResponse(Guid.NewGuid());

    public async Task<SalesOrderDetailsResponse?> GetOrderAsync(
        GetSalesOrderRequest req,
        CancellationToken ct) =>
        new SalesOrderDetailsResponse(
            new SalesOrderDto
            {
                Id = Guid.NewGuid(),
                OrderNumber = "MOCK-101",
                TotalAmount = 1500m,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Open"
            });

    // ---------------- INVOICE ----------------
    async Task<InvoiceDto?> IInvoiceService.GetByIdAsync(Guid id) =>
        new InvoiceDto { Id = id };

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize) =>
        new List<InvoiceDto>();

    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto)
    {
        dto.Id = Guid.NewGuid();
        return dto;
    }

    public async Task<InvoiceDto> CreateFromOrderAsync(Guid orderId) =>
        new InvoiceDto { Id = Guid.NewGuid() };

    // 🔥 FIXED MISSING METHODS
    public async Task<decimal> GetTotalOutstandingReceivablesAsync() =>
        50000m;

    public async Task<decimal> GetTotalMonthlySalesAsync() =>
        120000m;

    public async Task<decimal> GetOutstandingReceivablesAsync() => 50000m;

    public async Task<decimal> GetOutstandingReceivablesAsync(string userId) => 12000m;

    public async Task<decimal> GetMonthlySalesAsync(string userId) => 8500m;

    public async Task<int> CountOverdueInvoicesAsync() => 5;

    public async Task<int> CountInvoicesGeneratedAsync() => 120;

    public async Task<int> CountInvoicesDueAsync(string userId) => 3;

    public async Task<int> CountHighRiskAccountsAsync() => 2;
}

// CODE END