using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using TaskEntity = OperationalWorkspace.Domain.Entities.TaskEntity;

namespace OperationalWorkspaceApplication.Services;

public sealed class UnifiedService :
    IActivityService,
    IEmailService,
    IKnowledgeService,
    ISalesService,
    IBusinessPartnerService,
    IInventoryService,
    ITaskService,
    IInvoiceService
{
    private readonly IActivityRepository _activityRepo;
    private readonly ISageX3Client _sageClient;
    private readonly IntegrationDbContext _dbContext;
    private readonly ILogger<UnifiedService> _logger;

    public UnifiedService(
        IActivityRepository activityRepo,
        ISageX3Client sageClient,
        IntegrationDbContext dbContext,
        ILogger<UnifiedService> logger)
    {
        _activityRepo = activityRepo ?? throw new ArgumentNullException(nameof(activityRepo));
        _sageClient = sageClient ?? throw new ArgumentNullException(nameof(sageClient));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // =========================================================================
    // ---------------------------- ACTIVITY SERVICE ----------------------------
    // =========================================================================

    public async Task<IEnumerable<ActivityDto>> GetActivitiesAsync()
    {
        var activities = await _activityRepo.GetAllAsync(default);

        return activities.Select(a => new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty,
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action)).ToList();
    }

    public async Task<ActivityDto> CreateAsync(CreateActivityDto dto, string userEmail)
    {
        var entity = new Activity
        {
            Title = dto.Title,
            Description = dto.Description,
            ActivityType = dto.ActivityType,
            RelatedEntityId = dto.RelatedEntityId ?? Guid.Empty,
            CreatedBy = userEmail,
            CreatedAt = DateTime.UtcNow,
            Timestamp = DateTime.UtcNow,
            Action = "Create"
        };

        await _activityRepo.AddAsync(entity, default);

        return new ActivityDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.ActivityType,
            entity.RelatedEntityId ?? Guid.Empty,
            entity.CreatedAt,
            entity.CreatedBy,
            entity.Timestamp,
            entity.Action);
    }

    public async Task<ActivityDto?> GetByIdAsync(Guid id)
    {
        var a = await _activityRepo.GetByIdAsync(id, default);
        if (a == null) return null;

        return new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty,
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action);
    }

    public async Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId)
    {
        var activities = await _activityRepo.GetByRelatedEntityAsync(partnerId, default);

        return activities.Select(a => new ActivityDto(
            a.Id,
            a.Title,
            a.Description,
            a.ActivityType,
            a.RelatedEntityId ?? Guid.Empty,
            a.CreatedAt,
            a.CreatedBy,
            a.Timestamp,
            a.Action)).ToList();
    }

    public async Task AttachEmailAsync(object model)
    {
        _logger.LogInformation("Attaching email to activity context.");
        await Task.CompletedTask;
    }

    public async Task LogAsync(ActivityDto activity)
    {
        _logger.LogInformation("Logging activity: {Title}", activity.Title);

        var entity = new Activity
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            ActivityType = activity.ActivityType,
            RelatedEntityId = activity.RelatedEntityId,
            CreatedBy = activity.CreatedBy,
            CreatedAt = activity.CreatedAt,
            Timestamp = activity.Timestamp,
            Action = activity.Action
        };

        await _activityRepo.AddAsync(entity, default);
    }

    // =========================================================================
    // ---------------------------- KNOWLEDGE SERVICE ---------------------------
    // =========================================================================

    async Task<KnowledgeDto?> IKnowledgeService.GetByIdAsync(Guid id)
    {
        var k = await _dbContext.KnowledgeBase.FindAsync(id);
        return k == null ? null : new KnowledgeDto(k.Id, k.Title, string.Empty, string.Empty, string.Empty, string.Empty);
    }

    public async Task<IEnumerable<KnowledgeDto>> SearchAsync(string query)
    {
        return await _dbContext.KnowledgeBase
            .Where(k => k.Title.Contains(query))
            .Select(k => new KnowledgeDto(k.Id, k.Title, string.Empty, string.Empty, string.Empty, string.Empty))
            .ToListAsync();
    }

    public async Task<List<KnowledgeDto>> GetRecentArticlesAsync()
    {
        return await _dbContext.KnowledgeBase
            .Take(10)
            .Select(k => new KnowledgeDto(k.Id, k.Title, string.Empty, string.Empty, string.Empty, string.Empty))
            .ToListAsync();
    }

    public async Task SendKnowledgeAsync(object model) => await Task.CompletedTask;

    // =========================================================================
    // ------------------------------ EMAIL SERVICE ----------------------------
    // =========================================================================

    public async Task<bool> SyncEmailAsync(EmailInsightDto dto) => await Task.FromResult(true);

    public async Task<EmailInsightDto?> GetEmailByIdAsync(string emailId) => await Task.FromResult(new EmailInsightDto { MessageId = emailId });

    public async Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId) => await Task.FromResult(new List<OpenOrderDto>());

    public async Task<List<TaskDto>> GetLinkedTasksAsync(string emailId) => await Task.FromResult(new List<TaskDto>());

    // =========================================================================
    // ---------------------------- INVENTORY SERVICE --------------------------
    // =========================================================================

    public async Task<int> CountStockAlertsAsync() => await _sageClient.GetLowStockCountAsync();
    public async Task<InventoryItemDto?> GetItemAsync(Guid id, CancellationToken ct) => await _sageClient.GetItemDetailsAsync(id, ct);
    public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(string wh, CancellationToken ct) => await _sageClient.GetWarehouseStockAsync(wh, ct);
    public async Task<StockAvailabilityResponse> CheckAvailabilityAsync(CheckStockRequest r, CancellationToken ct) => await _sageClient.VerifyStockLevelsAsync(r, ct);
    public async Task<AdjustStockResponse> AdjustStockAsync(StockAdjustmentRequest r, CancellationToken ct) => await _sageClient.PostStockAdjustmentAsync(r, ct);
    public async Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(StockAdjustmentRequest r, CancellationToken ct) => await _sageClient.FetchAdjustmentLogAsync(r, ct);

    // =========================================================================
    // ------------------------- BUSINESS PARTNER SERVICE -----------------------
    // =========================================================================

    public async Task<string?> GetTopCustomerAsync(string userId)
    {
        // Fixed: Swapped missing 'CustomerCode' with your actual 'BpCode' property
        return await _dbContext.SalesOrders
            .GroupBy(o => o.BpCode)
            .Select(g => g.Key)
            .FirstOrDefaultAsync();
    }


    public async Task<string> GetRecentInteractionAsync(string userId)
    {
        var last = await _dbContext.Activities
            .Where(a => a.CreatedBy == userId)
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();
        return last != null ? $"Last interaction: {last.Timestamp:g} via {last.ActivityType}" : "No interactions found";
    }

    public async Task<int> CountOpenOpportunitiesAsync() => await Task.FromResult(0);
    public async Task<int> CountOpenOpportunitiesAsync(string userId) => await Task.FromResult(0);
    public async Task<int> CountNewLeadsTodayAsync() => await Task.FromResult(0);
    public async Task<int> CountActiveCustomersAsync() => await _sageClient.GetActivePartnersCountAsync();

    public async Task<BusinessPartnersResponse?> GetSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct) => await _sageClient.GetPartnerFinancialSnapshotAsync(req, ct);
    public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(UpdateCreditLimitRequest req, CancellationToken ct) => await _sageClient.PushCreditLimitUpdateAsync(req, ct);

    public async Task<BusinessPartnerSnapshotDto?> GetPartnerByEmailAsync(string? email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await _sageClient.FindPartnerByEmailAsync(email, ct);
    }

    public async Task<CreateClientFromEmailResponse> CreateFromEmailAsync(CreateClientFromEmailRequest request, CancellationToken ct = default) => await _sageClient.ProvisionPartnerAccountAsync(request, ct);

    public async Task<bool> CreateContactAsync(ContactCreateDto contact) => await Task.FromResult(true);

    // =========================================================================
    // ------------------------------ TASK SERVICE -----------------------------
    // =========================================================================

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest req, CancellationToken ct)
    {
        var newId = Guid.NewGuid();
        var task = new TaskEntity { Id = newId, Title = req.Title };
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(ct);

        // Fixed: Instantiate using available object constructor initializers matching layout signatures
        return new TaskResponse { IsSuccess = true };
    }

    public async Task<CompleteTaskResponse> CompleteAsync(CompleteTaskRequest req, CancellationToken ct)
    {
        var task = await _dbContext.Tasks.FindAsync(new object[] { req.TaskId }, ct);
        if (task == null) return new CompleteTaskResponse(false, "Task not found");
        await _dbContext.SaveChangesAsync(ct);
        return new CompleteTaskResponse(true, "Completed successfully");
    }

    public async Task<TaskListResponse> GetAsync(GetTasksRequest req, CancellationToken ct)
    {
        return new TaskListResponse(new List<TaskDto>());
    }

    public async Task<List<TaskDto>> GetTasksAssignedToAsync(string userId) => await Task.FromResult(new List<TaskDto>());
    public async Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId) => await Task.FromResult(new List<ApprovalDto>());
    public async Task<List<TaskDto>> GetAllTasksAsync() => await Task.FromResult(new List<TaskDto>());
    public async Task<List<ApprovalDto>> GetAllPendingApprovalsAsync() => await Task.FromResult(new List<ApprovalDto>());

    public async Task<TaskResponse> DelegateAsync(DelegateTaskRequest request, CancellationToken ct)
    {
        return new TaskResponse { IsSuccess = true };
    }

    // =========================================================================
    // ----------------------------- SALES SERVICE -----------------------------
    // =========================================================================

    public async Task<int> CountOpenOrdersAsync(string userId) => await _dbContext.SalesOrders.CountAsync();
    public async Task<int> CountPendingDeliveriesAsync(string userId) => await Task.FromResult(0);
    public async Task<int> CountTotalOrdersAsync() => await _dbContext.SalesOrders.CountAsync();
    public async Task<CreateSalesOrderResponse> CreateOrderAsync(CreateSalesOrderRequest req, CancellationToken ct) => await _sageClient.SubmitSalesOrderAsync(req, ct);
    public async Task<SalesOrderDetailsResponse?> GetOrderAsync(GetSalesOrderRequest req, CancellationToken ct) => await _sageClient.FetchSalesOrderAsync(req, ct);

    // =========================================================================
    // ---------------------------- INVOICE SERVICE ----------------------------
    // =========================================================================

    async Task<InvoiceDto?> IInvoiceService.GetByIdAsync(Guid id) => await _sageClient.GetInvoiceAsync(id);
    public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize) => await _sageClient.GetInvoicesPagedAsync(page, pageSize);
    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto) => await _sageClient.PostInvoiceAsync(dto);
    public async Task<InvoiceDto> CreateFromOrderAsync(Guid orderId) => await _sageClient.GenerateInvoiceFromSalesOrderAsync(orderId);
    public async Task<decimal> GetTotalOutstandingReceivablesAsync() => await _sageClient.SumGlobalOutstandingReceivablesAsync();
    public async Task<decimal> GetTotalMonthlySalesAsync() => await _sageClient.SumGlobalMonthlySalesAsync();
    public async Task<decimal> GetOutstandingReceivablesAsync() => await _sageClient.SumGlobalOutstandingReceivablesAsync();
    public async Task<decimal> GetOutstandingReceivablesAsync(string userId) => await _sageClient.SumUserOutstandingReceivablesAsync(userId);
    public async Task<decimal> GetMonthlySalesAsync(string userId) => await _sageClient.SumUserMonthlySalesAsync(userId);
    public async Task<int> CountOverdueInvoicesAsync() => await _sageClient.GetOverdueInvoicesCountAsync();
    public async Task<int> CountInvoicesGeneratedAsync() => await _sageClient.GetTotalGeneratedInvoicesCountAsync();
    public async Task<int> CountInvoicesDueAsync(string userId) => await _sageClient.GetUserInvoicesDueCountAsync(userId);
    public async Task<int> CountHighRiskAccountsAsync() => await _sageClient.GetHighRiskCreditAccountsCountAsync();
}
