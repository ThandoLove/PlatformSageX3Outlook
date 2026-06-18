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
    // 🗑️ CRITICAL DECOMMISSIONING COMPLETED
    // All mock Invoice generation methods, accounts receivable summations, and 
    // financial overdue counts have been completely scrubbed from this mock client!
    // =========================================================================
}
