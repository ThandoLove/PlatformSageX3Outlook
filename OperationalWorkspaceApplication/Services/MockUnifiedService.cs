using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceApplication.Services
{
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
        // =========================================================
        // IN-MEMORY ACTIVITY STORAGE
        // =========================================================
        private static readonly List<ActivityDto> _inMemoryActivities =
        [
            new ActivityDto(
                Guid.NewGuid(),
                "System Startup",
                "Workspace Initialized",
                "System",
                Guid.Empty,
                DateTime.UtcNow.AddMinutes(-10),
                "System",
                DateTime.UtcNow.AddMinutes(-10),
                "Initialize")
        ];

        // =========================================================
        // ACTIVITY SERVICE
        // =========================================================
        public async Task<IEnumerable<ActivityDto>> GetActivitiesAsync()
        {
            return await Task.FromResult(
                _inMemoryActivities
                    .OrderByDescending(a => a.Timestamp)
                    .ToList());
        }

        public async Task<ActivityDto?> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(
                _inMemoryActivities
                    .FirstOrDefault(a => a.Id == id));
        }

        public async Task<ActivityDto> CreateAsync(
            CreateActivityDto dto,
            string userEmail)
        {
            var newActivity =
                new ActivityDto(
                    Guid.NewGuid(),
                    dto.Title,
                    dto.Description,
                    dto.ActivityType,
                    dto.RelatedEntityId ?? Guid.Empty,
                    DateTime.UtcNow,
                    userEmail,
                    DateTime.UtcNow,
                    "Create");

            _inMemoryActivities.Insert(0, newActivity);

            return await Task.FromResult(newActivity);
        }

        public async Task<IEnumerable<ActivityDto>> GetByRelatedEntityAsync(Guid partnerId)
        {
            return await Task.FromResult(
                _inMemoryActivities
                    .Where(a => a.RelatedEntityId == partnerId)
                    .ToList());
        }

        public async Task AttachEmailAsync(object model)
        {
            await Task.CompletedTask;
        }

        public async Task LogAsync(ActivityDto activity)
        {
            if (!_inMemoryActivities.Any(a => a.Id == activity.Id))
            {
                _inMemoryActivities.Insert(0, activity);
            }

            await Task.CompletedTask;
        }

        // =========================================================
        // KNOWLEDGE SERVICE
        // =========================================================
        async Task<KnowledgeDto?> IKnowledgeService.GetByIdAsync(Guid id)
        {
            return await Task.FromResult(
                new KnowledgeDto(
                    id,
                    "Title",
                    "Body",
                    "Category",
                    "Summary",
                    "url"));
        }

        public async Task<IEnumerable<KnowledgeDto>> SearchAsync(string query)
        {
            return await Task.FromResult(
                new List<KnowledgeDto>());
        }

        public async Task<List<KnowledgeDto>> GetRecentArticlesAsync()
        {
            return await Task.FromResult(
                new List<KnowledgeDto>());
        }

        public async Task SendKnowledgeAsync(object model)
        {
            await Task.CompletedTask;
        }

        // =========================================================
        // EMAIL SERVICE (Hardened to match Guid Contract Signatures)
        // =========================================================
        public async Task<bool> SyncEmailAsync(EmailInsightDto dto)
        {
            return await Task.FromResult(true);
        }

        public async Task<EmailInsightDto?> GetEmailByIdAsync(Guid emailId)
        {
            return await Task.FromResult(
                new EmailInsightDto
                {
                    MessageId = emailId.ToString(),
                    Subject = "Mock Subject",
                    From = "test@example.com",
                    ReceivedAt = DateTime.UtcNow
                });
        }

        public async Task<EmailContextDto?> GetEmailContextAsync(Guid emailId)
        {
            var email =
                new EmailInsightDto
                {
                    MessageId = emailId.ToString(),
                    Subject = "Urgent: Verification Needed for Outstanding Order ORD-MOCK-7721",
                    From = "client-operator@postmantesting.com",
                    ReceivedAt = DateTime.UtcNow.AddMinutes(-5),
                    Message = "Please review outstanding system configurations for transaction reference token ORD-MOCK-7721 immediately.",
                    BusinessPartnerId = 9912,
                    BusinessPartnerName = "Mock Unified Testing Corporation",
                    BusinessPartnerCode = "BPC-MOCK-101",
                    OpenOrderCount = 2,
                    OpenOrderValue = 17400.50m,
                    OpenTaskCount = 1,
                    AssignedTaskCount = 1,
                    TotalOutstanding = 4500.00m,
                    OpenInvoiceCount = 1,
                    HasOverdueInvoices = true,
                    RiskLevel = "Normal",
                    AccountStatus = "Active",
                    IsOnCreditHold = false,
                    HasBackOrders = false,
                    HasLowStockImpact = false,
                    SnapshotGeneratedAtUtc = DateTime.UtcNow,
                    SenderEmail = "client-operator@postmantesting.com",
                    SenderName = "Mock Unified Testing Corporation",
                    To = "addin-operator@enterprise-domain.com",
                    AssignedUserId = "USR-MOCK-4401",
                    ClientId = Guid.NewGuid()
                };

            var linkedOrders =
                new List<LinkedOpenOrderDto>
                {
                    new LinkedOpenOrderDto
                    {
                        OrderId = "99121",
                        OrderNumber = "ORD-MOCK-7721",
                        Status = "Open",
                        CustomerName = "Mock Unified Testing Corporation"
                    },
                    new LinkedOpenOrderDto
                    {
                        OrderId = "99122",
                        OrderNumber = "ORD-MOCK-8834",
                        Status = "Pending",
                        CustomerName = "Mock Unified Testing Corporation"
                    }
                };

            var linkedTasks =
                new List<TaskDto>
                {
                    new TaskDto
                    {
                        Id = Guid.NewGuid(),
                        Title = "Review outstanding order verification"
                    }
                };

            var context =
                new EmailContextDto
                {
                    Email = email,
                    LinkedOrders = linkedOrders,
                    LinkedTasks = linkedTasks
                };

            return await Task.FromResult(context);
        }

        public async Task<List<OpenOrderDto>> GetLinkedOrdersAsync(Guid emailId)
        {
            return await Task.FromResult(
                new List<OpenOrderDto>());
        }

        public async Task<List<TaskDto>> GetLinkedTasksAsync(Guid emailId)
        {
            return await Task.FromResult(
                new List<TaskDto>());
        }

        // =========================================================
        // INVENTORY SERVICE
        // =========================================================
        public async Task<int> CountStockAlertsAsync() => 8;

        public async Task<InventoryItemDto?> GetItemAsync(
            Guid id,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new InventoryItemDto());
        }

        public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(
            string wh,
            CancellationToken ct)
        {
            return await Task.FromResult(
                (IReadOnlyList<InventoryItemDto>)
                new List<InventoryItemDto>());
        }

        public async Task<StockAvailabilityResponse> CheckAvailabilityAsync(
            CheckStockRequest r,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new StockAvailabilityResponse(true, 10));
        }

        public async Task<AdjustStockResponse> AdjustStockAsync(
            StockAdjustmentRequest r,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new AdjustStockResponse(true, "Success"));
        }

        public async Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(
            StockAdjustmentRequest r,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new StockAdjustmentDto());
        }

        // =========================================================
        // BUSINESS PARTNER SERVICE
        // =========================================================
        public async Task<string?> GetTopCustomerAsync(string userId)
        {
            return await Task.FromResult("Global Industries Ltd");
        }

        public async Task<string> GetRecentInteractionAsync(string userId)
        {
            return await Task.FromResult(
                "Last interaction: Yesterday via Email");
        }

        public async Task<int> CountOpenOpportunitiesAsync() => 15;
        public async Task<int> CountOpenOpportunitiesAsync(string userId) => 4;
        public async Task<int> CountNewLeadsTodayAsync() => 3;
        public async Task<int> CountActiveCustomersAsync() => 450;

        public async Task<BusinessPartnersResponse?> GetSnapshotAsync(
            GetBusinessPartnerSnapshotRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new BusinessPartnersResponse(
                    new BusinessPartnerSnapshotDto(
                        "C1000",
                        "Mock Corp",
                        500m,
                        5000m,
                        2,
                        1,
                        15000m,
                        DateTime.UtcNow)));
        }

        public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(
            UpdateCreditLimitRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new UpdateCreditLimitResponse(true));
        }

        public async Task<BusinessPartnerSnapshotDto?> GetPartnerByEmailAsync(
            string? email,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return await Task.FromResult(
                new BusinessPartnerSnapshotDto(
                    "C1000",
                    "Mock Corp",
                    500m,
                    5000m,
                    2,
                    1,
                    15000m,
                    DateTime.UtcNow)
                {
                    FullName = "Mock User",
                    IsLinkedToSage = true,
                    Timeline = new List<ActivityDto>()
                });
        }

        public async Task<CreateClientFromEmailResponse> CreateFromEmailAsync(
            CreateClientFromEmailRequest request,
            CancellationToken ct = default)
        {
            return await Task.FromResult(
                new CreateClientFromEmailResponse
                {
                    Id = Guid.NewGuid(),
                    Code = $"MOCK-{request.Email}"
                });
        }

        public async Task<bool> CreateContactAsync(ContactCreateDto contact)
        {
            await Task.Delay(50);
            return true;
        }

        // =========================================================
        // TASK SERVICE
        // =========================================================
        public async Task<TaskResponse> CreateAsync(
            CreateTaskRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new TaskResponse
                {
                    Id = Guid.NewGuid(),
                    IsSuccess = true
                });
        }

        public async Task<CompleteTaskResponse> CompleteAsync(
            CompleteTaskRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new CompleteTaskResponse(true, "Done"));
        }

        public async Task<TaskListResponse> GetAsync(
            GetTasksRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new TaskListResponse(
                    new List<TaskDto>()));
        }

        public async Task<List<TaskDto>> GetTasksAssignedToAsync(string userId)
        {
            return await Task.FromResult(
                new List<TaskDto>());
        }

        public async Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId)
        {
            return await Task.FromResult(
                new List<ApprovalDto>());
        }

        public async Task<List<TaskDto>> GetAllTasksAsync()
        {
            return await Task.FromResult(
                new List<TaskDto>());
        }

        public async Task<List<ApprovalDto>> GetAllPendingApprovalsAsync()
        {
            return await Task.FromResult(
                new List<ApprovalDto>());
        }

        public async Task<TaskResponse> DelegateAsync(
            DelegateTaskRequest request,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new TaskResponse
                {
                    IsSuccess = true,
                    Id = request.TaskId
                });
        }

        // =========================================================
        // SALES SERVICE
        // =========================================================
        public async Task<int> CountOpenOrdersAsync(string userId) => 0;
        public async Task<int> CountPendingDeliveriesAsync(string userId) => 0;
        public async Task<int> CountTotalOrdersAsync() => 0;

        public async Task<CreateSalesOrderResponse> CreateOrderAsync(
            CreateSalesOrderRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new CreateSalesOrderResponse(Guid.NewGuid()));
        }

        public async Task<SalesOrderDetailsResponse?> GetOrderAsync(
            GetSalesOrderRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new SalesOrderDetailsResponse(
                    new SalesOrderDto
                    {
                        Id = Guid.NewGuid(),
                        OrderNumber = "MOCK-101",
                        TotalAmount = 1500m,
                        OrderDate = DateTime.UtcNow,
                        OrderStatus = "Open"
                    }));
        }

        // =========================================================
        // INVOICE SERVICE
        // =========================================================
        async Task<InvoiceDto?> IInvoiceService.GetByIdAsync(Guid id)
        {
            return await Task.FromResult(
                new InvoiceDto
                {
                    Id = id
                });
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync(
            int page,
            int pageSize)
        {
            return await Task.FromResult(
                (IEnumerable<InvoiceDto>)
                new List<InvoiceDto>());
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto)
        {
            dto.Id = Guid.NewGuid();
            return await Task.FromResult(dto);
        }

        public async Task<InvoiceDto> CreateFromOrderAsync(Guid orderId)
        {
            return await Task.FromResult(
                new InvoiceDto
                {
                    Id = Guid.NewGuid()
                });
        }

        public async Task<decimal> GetTotalOutstandingReceivablesAsync() => 50000m;
        public async Task<decimal> GetTotalMonthlySalesAsync() => 120000m;
        public async Task<decimal> GetOutstandingReceivablesAsync() => 50000m;
        public async Task<decimal> GetOutstandingReceivablesAsync(string userId) => 12000m;
        public async Task<decimal> GetMonthlySalesAsync(string userId) => 8500m;
        public async Task<int> CountOverdueInvoicesAsync() => 5;
        public async Task<int> CountInvoicesGeneratedAsync() => 120;
        public async Task<int> CountInvoicesDueAsync(string userId) => 3;
        public async Task<int> CountHighRiskAccountsAsync() => 2;
    }
}
