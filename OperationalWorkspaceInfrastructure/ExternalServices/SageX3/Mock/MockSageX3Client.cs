using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

    public Task<CreateSalesOrderResponse> SubmitSalesOrderAsync(CreateSalesOrderRequest req, CancellationToken ct = default)
        => Task.FromResult(new CreateSalesOrderResponse(Guid.NewGuid()));

    public Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct = default)
        => Task.FromResult<SalesOrderDetailsResponse?>(new SalesOrderDetailsResponse(new SalesOrderDto { Id = Guid.NewGuid(), OrderNumber = "MOCK-101", TotalAmount = 1500m, OrderStatus = "Open" }));

    // =========================================================================
    // 🟢 INVOICE MEMBERS (🚀 FIXED FROM BACKEND NETWORK COMPILATION ERROR)
    // =========================================================================

    public Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult<InvoiceDto?>(new InvoiceDto
        {
            Id = id,
            InvoiceNumber = "INV-MOCK-771",
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
        // 🚀 FIXED: Stripped out the broken SafeGetAsync query dependency. 
        // Instantly dispatches a clean, structured mock data collection array to avoid compiler crashes.
        var mockPagedInvoicesCollection = new List<InvoiceDto>
        {
            new InvoiceDto { Id = Guid.NewGuid(), InvoiceNumber = "INV-MOCK-001", CustomerName = "Tech Innovations Inc.", TotalAmount = 12500m, OutstandingAmount = 0m, Status = "Paid", IssueDate = DateTime.UtcNow.AddDays(-30), DueDate = DateTime.UtcNow },
            new InvoiceDto { Id = Guid.NewGuid(), InvoiceNumber = "INV-MOCK-002", CustomerName = "Global Systems Ltd", TotalAmount = 8400m, OutstandingAmount = 8400m, Status = "Unpaid", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(15) }
        };

        return Task.FromResult<IEnumerable<InvoiceDto>>(mockPagedInvoicesCollection);
    }

    public Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default)
        => Task.FromResult(5);

    public Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default)
        => Task.FromResult(120);

    public Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(3);

    public Task<int> GetUserTotalInvoicesGeneratedCountAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(120);
}
