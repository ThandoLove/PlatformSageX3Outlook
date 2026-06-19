using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public interface ISageX3Client
{
    Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default);

    // Business Partner Operations
    Task<int> GetActivePartnersCountAsync(CancellationToken ct = default);
    Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct = default);
    Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct = default);
    Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct = default);
    Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct = default);

    // Sales Operations
    Task<CreateSalesOrderResponse> SubmitSalesOrderAsync(CreateSalesOrderRequest req, CancellationToken ct = default);
    Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct = default);

    // =========================================================================
    // 🗑️ CRITICAL DECOMMISSIONING COMPLETED
    // All local mutable Invoice calculation, summary, and transaction creation
    // interface methods have been completely scrubbed from this contract!
    // =========================================================================

    // invoices

    Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default);
    Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default);
    Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default);
    Task<int> GetUserTotalInvoicesGeneratedCountAsync(string userId, CancellationToken ct = default);

}
