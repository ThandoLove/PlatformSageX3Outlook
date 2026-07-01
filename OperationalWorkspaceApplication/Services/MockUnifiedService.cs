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

        // STEP 1 — Added Demo Data Storage Fields
        private static readonly List<BusinessPartnerSnapshotDto> _customers = new();
        private static readonly List<TaskDto> _tasks = new();
        private static readonly List<SalesOrderDto> _orders = new();
        private static readonly List<InvoiceDto> _invoices = new();

        // STEP 2 — Service Constructor for Lazy Initialisation
        public MockUnifiedService()
        {
            if (_customers.Count == 0)
            {
                SeedDemoData();
            }
        }

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

        // STEP 4 — Replaced with in-memory collection tracking loop
        public async Task<int> CountActiveCustomersAsync()
        {
            return await Task.FromResult(_customers.Count);
        }

        public async Task<BusinessPartnersResponse?> GetSnapshotAsync(
            GetBusinessPartnerSnapshotRequest req,
            CancellationToken ct)
        {
            return await Task.FromResult(
                new BusinessPartnersResponse(
                    new BusinessPartnerSnapshotDto(
                        "C1000",
                        "Mock Corp",
                        25000m,
                        5000m,
                        2,
                        1,
                        15000m,
                        0m, // 8th decimal argument
                        DateTime.UtcNow) // 9th LastContactDate argument
                    {
                        FullName = "Mock Corp",
                        IsLinkedToSage = true,
                        Timeline = new List<ActivityDto>()
                    }));
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

            // Extract the user handle prefix from the email to dynamically match against FullName
            var userHandle = email.Split('@')[0];

            // Look up the customer dynamically from our seeded _customers list via FullName safely
            var partner = _customers.FirstOrDefault(x =>
                x.FullName != null && x.FullName.Contains(userHandle, StringComparison.OrdinalIgnoreCase));

            if (partner != null)
            {
                return await Task.FromResult(partner);
            }

            // Safe static fallback object matching the exact 9-argument signature if no matching entry is found
            return await Task.FromResult(
                new BusinessPartnerSnapshotDto(
                    "C1000",
                    "Mock Corp",
                    100000m,
                    5000m,
                    2,
                    1,
                    15000m,
                    0m, // 8th argument (decimal)
                    DateTime.UtcNow) // 9th argument (LastContactDate)
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

        // STEP 4 — Replaced to reference the seeded demo tasks collection
        public async Task<List<TaskDto>> GetAllTasksAsync()
        {
            return await Task.FromResult(_tasks);
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

        // STEP 4 — Replaced to count total items from the memory order list
        public async Task<int> CountOpenOrdersAsync(string userId)
        {
            return await Task.FromResult(_orders.Count);
        }

        public async Task<int> CountPendingDeliveriesAsync(string userId) => 0;

        // STEP 4 — Replaced to return total count of tracking order entities
        public async Task<int> CountTotalOrdersAsync()
        {
            return await Task.FromResult(_orders.Count);
        }

        public async Task<Guid> CreateOrderAsync(string bpCode, string customerRef, decimal totalAmount, CancellationToken ct)
        {
            await Task.Delay(50, ct);
            throw new NotSupportedException("Order creation is not supported inside the workspace application. Please create mutations directly in Sage X3.");
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
        // STEP 4 — Replaced with paginated query logic matching the in-memory invoices list
        public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize)
        {
            return await Task.FromResult(
                _invoices.Skip((page - 1) * pageSize)
                         .Take(pageSize));
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

        // STEP 4 — Replaced to evaluate overdue flags dynamically against live collection items
        public async Task<int> CountOverdueInvoicesAsync()
        {
            return await Task.FromResult(
                _invoices.Count(i => i.IsOverdue));
        }

        public async Task<int> CountInvoicesGeneratedAsync() => 120;
        public async Task<int> CountInvoicesDueAsync(string userId) => 3;

        public async Task<decimal> GetOutstandingInvoiceValueAsync()
        {
            return await Task.FromResult(50000m);
        }

        public async Task<decimal> GetUserOutstandingInvoiceValueAsync(string userId)
        {
            return await Task.FromResult(12000m);
        }

        public async Task<decimal> GetCurrentMonthInvoiceValueAsync()
        {
            return await Task.FromResult(120000m);
        }

        public async Task<decimal> GetUserCurrentMonthInvoiceValueAsync(string userId)
        {
            return await Task.FromResult(8500m);
        }

        // =========================================================================
        // 🧪 MOCK COMPLIANCE LAYER STUB
        // =========================================================================
        public async Task<bool> CreateNewSageClientAsync(ClientDto clientDto)
        {
            return await Task.FromResult(true);
        }
        // =========================================================================
        // STEP 3 — DEMO ENVIRONMENT DATA SEEDER ENGINE
        // =========================================================================
        private static void SeedDemoData()
        {
            var random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                var id = Guid.NewGuid();

                //------------------------------------------------
                // Customers
                //------------------------------------------------
                _customers.Add(new BusinessPartnerSnapshotDto(
                    $"BP{i:0000}",
                    $"Customer {i}",
                    100000m,
                    (decimal)random.Next(5000, 50000),
                    random.Next(0, 8),
                    random.Next(0, 3),
                    (decimal)random.Next(0, 12000),
                    0m, // 8th argument (decimal placeholder)
                    DateTime.UtcNow.AddDays(-random.Next(30))) // 9th LastContactDate argument
                {
                    Id = id,
                    FullName = $"Customer {i}",
                    AssignedRep = "Admin Operator",
                    IsLinkedToSage = true,
                    Location = "Harare"
                });

                //------------------------------------------------
                // Orders
                //------------------------------------------------
                if (i <= 8)
                {
                    _orders.Add(new SalesOrderDto
                    {
                        Id = Guid.NewGuid(),
                        OrderNumber = $"SO-{1000 + i}",
                        BusinessPartnerId = id,
                        BusinessPartnerCode = $"BP{i:0000}",
                        BusinessPartnerName = $"Customer {i}",
                        TotalAmount = random.Next(1000, 25000),
                        OutstandingAmount = random.Next(500, 10000),
                        OrderStatus = "Open",
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-i),
                        RequestedDeliveryDate = DateTime.UtcNow.AddDays(i)
                    });
                }

                //------------------------------------------------
                // Invoices
                //------------------------------------------------
                if (i <= 10)
                {
                    _invoices.Add(new InvoiceDto
                    {
                        Id = Guid.NewGuid(),
                        InvoiceNumber = $"INV-{5000 + i}",
                        CustomerName = $"Customer {i}",
                        OutstandingAmount = random.Next(500, 8000),
                        Balance = random.Next(500, 8000),
                        DueDate = DateTime.Today.AddDays(random.Next(-10, 15)),
                        IssueDate = DateTime.Today.AddDays(-20),
                        IsOverdue = i % 3 == 0
                    });
                }

                //------------------------------------------------
                // Tasks
                //------------------------------------------------
                if (i <= 12)
                {
                    _tasks.Add(new TaskDto
                    {
                        Id = Guid.NewGuid(),
                        Title = $"Follow up Customer {i}",
                        CompanyName = $"Customer {i}",
                        AssignedTo = "Admin Operator",
                        StatusDescription = "Open",
                        CreatedDate = DateTime.Now.AddDays(-i),
                        UpdatedDate = DateTime.Now
                    });
                }

                //------------------------------------------------
                // Activities
                //------------------------------------------------
                if (i <= 15)
                {
                    _inMemoryActivities.Add(
                        new ActivityDto(
                            Guid.NewGuid(),
                            $"Customer Visit {i}",
                            $"Visited Customer {i}",
                            "Visit",
                            id,
                            DateTime.UtcNow.AddDays(-i),
                            "Admin Operator",
                            DateTime.UtcNow.AddDays(-i),
                            "Create"));
                }
            }
        }
    }
}
