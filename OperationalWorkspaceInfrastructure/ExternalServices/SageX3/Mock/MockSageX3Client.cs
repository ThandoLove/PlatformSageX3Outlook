using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspaceApplication.Interfaces;


namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;

public class MockSageX3Client : ISageX3Client
{
    public Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default)
        => Task.FromResult($"MOCK_CUSTOMER_{bpCode}");

    // =========================================================================
    // 🟢 MOCK BUSINESS PARTNER MEMBERS
    // =========================================================================

    public Task<int> GetActivePartnersCountAsync(CancellationToken ct = default)
        => Task.FromResult(450);

    public Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct = default)
        => Task.FromResult<BusinessPartnersResponse?>(new BusinessPartnersResponse(new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 0m, 0m, 2, 0, 0m, DateTime.UtcNow)));

    public Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct = default)
        => Task.FromResult(new UpdateCreditLimitResponse(true));

    public Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult<BusinessPartnerSnapshotDto?>(new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 0m, 0m, 2, 0, 0m, DateTime.UtcNow) { FullName = "Mock User", IsLinkedToSage = true });

    public Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct = default)
        => Task.FromResult(new CreateClientFromEmailResponse { Id = Guid.NewGuid(), Code = $"MOCK-{request.Email}" });

    // =========================================================================
    // 🟢 MOCK SALES MEMBERS
    // =========================================================================

    public async Task<Guid> CreateSalesOrderAsync(string bpCode, string customerRef, decimal totalAmount, CancellationToken ct)
    {
        await Task.Delay(10, ct);
        return Guid.Empty;
    }

    public Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct = default)
        => Task.FromResult<SalesOrderDetailsResponse?>(new SalesOrderDetailsResponse(new SalesOrderDto { Id = Guid.NewGuid(), OrderNumber = "MOCK-101", TotalAmount = 1500m, OrderStatus = "Open" }));

    // =========================================================================
    // 📄 READ-ONLY INVOICE LOOKUPS
    // =========================================================================

    public Task<InvoiceDto?> GetInvoiceAsync(string invoiceNumber, CancellationToken ct = default)
    {
        return Task.FromResult<InvoiceDto?>(new InvoiceDto
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            CustomerName = "Mock Client Industries",
            TotalAmount = 4500.00m,
            OutstandingAmount = 1200.00m,
            IssueDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(25),
            Status = "Unpaid"
        });
    }

    public Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var mockPagedInvoicesCollection = new List<InvoiceDto>
        {
            new InvoiceDto { Id = Guid.NewGuid(), InvoiceNumber = "INV-MOCK-001", CustomerName = "Tech Innovations Inc.", TotalAmount = 12500m, OutstandingAmount = 0m, Status = "Paid", IssueDate = DateTime.UtcNow.AddDays(-30), DueDate = DateTime.UtcNow },
            new InvoiceDto { Id = Guid.NewGuid(), InvoiceNumber = "INV-MOCK-002", CustomerName = "Global Systems Ltd", TotalAmount = 8400m, OutstandingAmount = 8400m, Status = "Unpaid", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(15) }
        };

        return Task.FromResult<IEnumerable<InvoiceDto>>(mockPagedInvoicesCollection);
    }

    // =========================================================================
    // 📊 GLOBAL INVOICE METRICS (DASHBOARD / ADMIN KPIS)
    // =========================================================================

    public Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default)
        => Task.FromResult(5);

    public Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default)
        => Task.FromResult(120);

    public Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(3);

    public Task<int> GetUserTotalInvoicesGeneratedCountAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(120);

    public Task<decimal> GetOutstandingInvoiceValueAsync(CancellationToken ct = default)
    {
        return Task.FromResult(185000m);
    }

    public Task<decimal> GetUserOutstandingInvoiceValueAsync(string userId, CancellationToken ct = default)
    {
        return Task.FromResult(42000m);
    }

    public Task<decimal> GetCurrentMonthInvoiceValueAsync(CancellationToken ct = default)
    {
        return Task.FromResult(76000m);
    }

    public Task<decimal> GetUserCurrentMonthInvoiceValueAsync(string userId, CancellationToken ct = default)
    {
        return Task.FromResult(18000m);
    }

    // =========================================================================
    // 🔥 BP-SPECIFIC INVOICE METRICS (CUSTOMER CONTEXT / SNAPSHOTS)
    // =========================================================================

    public Task<int> GetOverdueInvoicesCountAsync(string bpCode, CancellationToken ct = default)
        => Task.FromResult(2);

    public Task<decimal> GetOutstandingInvoiceValueAsync(string bpCode, CancellationToken ct = default)
        => Task.FromResult(42000m);

    public Task<decimal> GetCurrentMonthInvoiceValueAsync(string bpCode, CancellationToken ct = default)
        => Task.FromResult(18000m);
}
