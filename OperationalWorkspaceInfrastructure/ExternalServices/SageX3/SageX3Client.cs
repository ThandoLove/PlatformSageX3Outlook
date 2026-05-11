using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.Exceptions;
using OperationalWorkspaceInfrastructure.Http;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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

public class SageX3Client : ISageX3Client
{
    private readonly ISageAuthService _authService;
    private readonly ISageHttpClient _httpClient;
    private readonly string _baseUrl;

    public SageX3Client(ISageAuthService authService, ISageHttpClient httpClient, string baseUrl)
    {
        _authService = authService;
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetAccessTokenAsync(cancellationToken);
        var response = await _httpClient.GetAsync($"{_baseUrl}/customers/{bpCode}", token, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new SageIntegrationException("Failed to retrieve customer data");

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    // =========================================================================
    // 🚀 DORMANT IMPLEMENTATIONS: Fallbacks to make UnifiedService compile instantly
    // =========================================================================

    public async Task<int> GetLowStockCountAsync(CancellationToken ct) => await Task.FromResult(0);
    public async Task<InventoryItemDto?> GetItemDetailsAsync(Guid id, CancellationToken ct) => await Task.FromResult<InventoryItemDto?>(null);
    public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseStockAsync(string wh, CancellationToken ct) => await Task.FromResult(new List<InventoryItemDto>());
    public async Task<StockAvailabilityResponse> VerifyStockLevelsAsync(CheckStockRequest r, CancellationToken ct) => await Task.FromResult(new StockAvailabilityResponse(false, 0));
    public async Task<AdjustStockResponse> PostStockAdjustmentAsync(StockAdjustmentRequest r, CancellationToken ct) => await Task.FromResult(new AdjustStockResponse(false, "Standby"));
    public async Task<StockAdjustmentDto> FetchAdjustmentLogAsync(StockAdjustmentRequest r, CancellationToken ct) => await Task.FromResult(new StockAdjustmentDto());
    public async Task<int> GetActivePartnersCountAsync(CancellationToken ct) => await Task.FromResult(0);
    public async Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct) => await Task.FromResult<BusinessPartnersResponse?>(null);
    public async Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct) => await Task.FromResult(new UpdateCreditLimitResponse(false));
    public async Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct) => await Task.FromResult<BusinessPartnerSnapshotDto?>(null);
    public async Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct) => await Task.FromResult(new CreateClientFromEmailResponse());
    public async Task<CreateSalesOrderResponse> SubmitSalesOrderAsync(CreateSalesOrderRequest req, CancellationToken ct) => await Task.FromResult(new CreateSalesOrderResponse(Guid.Empty));
    public async Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct) => await Task.FromResult<SalesOrderDetailsResponse?>(null);
    public async Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct) => await Task.FromResult<InvoiceDto?>(null);
    public async Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct) => await Task.FromResult(new List<InvoiceDto>());
    public async Task<InvoiceDto> PostInvoiceAsync(InvoiceDto dto, CancellationToken ct) => await Task.FromResult(dto);
    public async Task<InvoiceDto> GenerateInvoiceFromSalesOrderAsync(Guid orderId, CancellationToken ct) => await Task.FromResult(new InvoiceDto());
    public async Task<decimal> SumGlobalOutstandingReceivablesAsync(CancellationToken ct) => await Task.FromResult(0m);
    public async Task<decimal> SumGlobalMonthlySalesAsync(CancellationToken ct) => await Task.FromResult(0m);
    public async Task<decimal> SumUserOutstandingReceivablesAsync(string userId, CancellationToken ct) => await Task.FromResult(0m);
    public async Task<decimal> SumUserMonthlySalesAsync(string userId, CancellationToken ct) => await Task.FromResult(0m);
    public async Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct) => await Task.FromResult(0);
    public async Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct) => await Task.FromResult(0);
    public async Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct) => await Task.FromResult(0);
    public async Task<int> GetHighRiskCreditAccountsCountAsync(CancellationToken ct) => await Task.FromResult(0);
}
