
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

    // Inventory Operations
    Task<int> GetLowStockCountAsync(CancellationToken ct = default);
    Task<InventoryItemDto?> GetItemDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<InventoryItemDto>> GetWarehouseStockAsync(string wh, CancellationToken ct = default);
    Task<StockAvailabilityResponse> VerifyStockLevelsAsync(CheckStockRequest r, CancellationToken ct = default);
    Task<AdjustStockResponse> PostStockAdjustmentAsync(StockAdjustmentRequest r, CancellationToken ct = default);
    Task<StockAdjustmentDto> FetchAdjustmentLogAsync(StockAdjustmentRequest r, CancellationToken ct = default);

    // Business Partner Operations
    Task<int> GetActivePartnersCountAsync(CancellationToken ct = default);
    Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct = default);
    Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct = default);
    Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct = default);
    Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct = default);

    // Sales Operations
    Task<CreateSalesOrderResponse> SubmitSalesOrderAsync(CreateSalesOrderRequest req, CancellationToken ct = default);
    Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct = default);

    // Invoice Operations
    Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<InvoiceDto> PostInvoiceAsync(InvoiceDto dto, CancellationToken ct = default);
    Task<InvoiceDto> GenerateInvoiceFromSalesOrderAsync(Guid orderId, CancellationToken ct = default);
    Task<decimal> SumGlobalOutstandingReceivablesAsync(CancellationToken ct = default);
    Task<decimal> SumGlobalMonthlySalesAsync(CancellationToken ct = default);
    Task<decimal> SumUserOutstandingReceivablesAsync(string userId, CancellationToken ct = default);
    Task<decimal> SumUserMonthlySalesAsync(string userId, CancellationToken ct = default);
    Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default);
    Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default);
    Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default);
    Task<int> GetHighRiskCreditAccountsCountAsync(CancellationToken ct = default);
}
