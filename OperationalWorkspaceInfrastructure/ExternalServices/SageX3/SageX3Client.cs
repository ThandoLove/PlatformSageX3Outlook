using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.Exceptions;
using OperationalWorkspaceInfrastructure.Http;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Net.Http;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public class SageX3Client : ISageX3Client
{
    private readonly ISageAuthService _authService;
    private readonly ISageHttpClient _httpClient;

    public SageX3Client(
        ISageAuthService authService,
        ISageHttpClient httpClient)
    {
        _authService = authService;
        _httpClient = httpClient;
    }

    public async Task<string> GetCustomerDataAsync(
        string bpCode,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetAccessTokenAsync(cancellationToken);

        var response = await _httpClient.GetAsync(
            $"/customers/{bpCode}",
            token,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new SageIntegrationException("Failed to retrieve customer data");

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    // ======================================================
    // SAFE DORMANT IMPLEMENTATIONS (UNCHANGED)
    // ======================================================

    public Task<int> GetLowStockCountAsync(CancellationToken ct)
        => Task.FromResult(0);

    public Task<InventoryItemDto?> GetItemDetailsAsync(Guid id, CancellationToken ct)
        => Task.FromResult<InventoryItemDto?>(null);

    public Task<IReadOnlyList<InventoryItemDto>> GetWarehouseStockAsync(string wh, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<InventoryItemDto>>(new List<InventoryItemDto>());

    public Task<StockAvailabilityResponse> VerifyStockLevelsAsync(CheckStockRequest r, CancellationToken ct)
        => Task.FromResult(new StockAvailabilityResponse(false, 0));

    public Task<AdjustStockResponse> PostStockAdjustmentAsync(StockAdjustmentRequest r, CancellationToken ct)
        => Task.FromResult(new AdjustStockResponse(false, "Standby"));

    public Task<StockAdjustmentDto> FetchAdjustmentLogAsync(StockAdjustmentRequest r, CancellationToken ct)
        => Task.FromResult(new StockAdjustmentDto());

    public Task<int> GetActivePartnersCountAsync(CancellationToken ct)
        => Task.FromResult(0);

    public Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct)
        => Task.FromResult<BusinessPartnersResponse?>(null);

    public Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct)
        => Task.FromResult(new UpdateCreditLimitResponse(false));

    public Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct)
        => Task.FromResult<BusinessPartnerSnapshotDto?>(null);

    public Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct)
        => Task.FromResult(new CreateClientFromEmailResponse());

    public Task<CreateSalesOrderResponse> SubmitSalesOrderAsync(CreateSalesOrderRequest req, CancellationToken ct)
        => Task.FromResult(new CreateSalesOrderResponse(Guid.Empty));

    public Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct)
        => Task.FromResult<SalesOrderDetailsResponse?>(null);

    public Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct)
        => Task.FromResult<InvoiceDto?>(null);

    public Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct)
        => Task.FromResult<IEnumerable<InvoiceDto>>(new List<InvoiceDto>());

    public Task<InvoiceDto> PostInvoiceAsync(InvoiceDto dto, CancellationToken ct)
        => Task.FromResult(dto);

    public Task<InvoiceDto> GenerateInvoiceFromSalesOrderAsync(Guid orderId, CancellationToken ct)
        => Task.FromResult(new InvoiceDto());

    public Task<decimal> SumGlobalOutstandingReceivablesAsync(CancellationToken ct)
        => Task.FromResult(0m);

    public Task<decimal> SumGlobalMonthlySalesAsync(CancellationToken ct)
        => Task.FromResult(0m);

    public Task<decimal> SumUserOutstandingReceivablesAsync(string userId, CancellationToken ct)
        => Task.FromResult(0m);

    public Task<decimal> SumUserMonthlySalesAsync(string userId, CancellationToken ct)
        => Task.FromResult(0m);

    public Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct)
        => Task.FromResult(0);

    public Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct)
        => Task.FromResult(0);

    public Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct)
        => Task.FromResult(0);

    public Task<int> GetHighRiskCreditAccountsCountAsync(CancellationToken ct)
        => Task.FromResult(0);
}