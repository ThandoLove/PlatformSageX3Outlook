using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;


namespace OperationalWorkspaceApplication.Interfaces;

public interface ISageX3Client
{
    Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default);

    // Business Partner Operations
    Task<int> GetActivePartnersCountAsync(CancellationToken ct = default);
    Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct = default);
    Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct = default);
    Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct = default);
    Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct = default);

    // Sales Operations (Read-Only Lookups)
    Task<Guid> CreateSalesOrderAsync(string bpCode, string customerRef, decimal totalAmount, CancellationToken ct);
    Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct = default);

    // =========================================================================
    // 📊 GLOBAL INVOICE METRICS (DASHBOARD / ADMIN KPIS)
    // =========================================================================
    Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default);
    Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default);
    Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default);
    Task<int> GetUserTotalInvoicesGeneratedCountAsync(string userId, CancellationToken ct = default);
    Task<decimal> GetOutstandingInvoiceValueAsync(CancellationToken ct = default);
    Task<decimal> GetUserOutstandingInvoiceValueAsync(string userId, CancellationToken ct = default);
    Task<decimal> GetCurrentMonthInvoiceValueAsync(CancellationToken ct = default);
    Task<decimal> GetUserCurrentMonthInvoiceValueAsync(string userId, CancellationToken ct = default);

    // =========================================================================
    // 🔥 BP-SPECIFIC INVOICE METRICS (CUSTOMER CONTEXT / BUSINESS PARTNER SNAPSHOT)
    // =========================================================================
    Task<int> GetOverdueInvoicesCountAsync(string bpCode, CancellationToken ct = default);
    Task<decimal> GetOutstandingInvoiceValueAsync(string bpCode, CancellationToken ct = default);
    Task<decimal> GetCurrentMonthInvoiceValueAsync(string bpCode, CancellationToken ct = default);

    // =========================================================================
    // 📄 READ-ONLY INVOICE LOOKUPS
    // =========================================================================
    Task<InvoiceDto?> GetInvoiceAsync(string invoiceNumber, CancellationToken ct = default);
    Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct = default);
}
