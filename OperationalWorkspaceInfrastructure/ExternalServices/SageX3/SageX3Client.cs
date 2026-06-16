using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.Exceptions;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.SageConfiguration;
using OperationalWorkspaceInfrastructure.Http;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Net;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

/// <summary>
/// Sage X3 REST client. Endpoint paths are defined in <see cref="SageX3RestEndpoints"/>.
/// Calls fail gracefully until Sage X3 is configured and reachable.
/// </summary>
public class SageX3Client : ISageX3Client
{
    private readonly ISageAuthService _authService;
    private readonly ISageHttpClient _httpClient;
    private readonly SageSettings _settings;
    private readonly ILogger<SageX3Client> _logger;

    public SageX3Client(
        ISageAuthService authService,
        ISageHttpClient httpClient,
        IOptions<SageSettings> settings,
        ILogger<SageX3Client> logger)
    {
        _authService = authService;
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    private string BaseUrl => _settings.RestBaseUrl.TrimEnd('/');

    private async Task<HttpResponseMessage?> SafeGetAsync(string relativePath, CancellationToken ct)
    {
        try
        {
            var token = await _authService.GetAccessTokenAsync(ct);
            var url = $"{BaseUrl}/{relativePath.TrimStart('/')}";
            var response = await _httpClient.GetAsync(url, token, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Sage X3 GET {Path} returned {Status}", relativePath, response.StatusCode);
                return null;
            }
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sage X3 GET {Path} failed (not connected or misconfigured)", relativePath);
            return null;
        }
    }

    public async Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default)
    {
        var response = await SafeGetAsync(SageX3RestEndpoints.CustomerByCode(bpCode), cancellationToken);
        if (response == null)
            throw new SageIntegrationException("Failed to retrieve customer data");

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct = default)
    {
        var response = await SafeGetAsync(SageX3RestEndpoints.ContactByEmail(email), ct);
        if (response == null) return null;

        var body = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(body)) return null;

        return new BusinessPartnerSnapshotDto(
            BpCode: string.Empty,
            Company: email,
            CreditLimit: 0,
            TotalOutstanding: 0,
            OpenOrderCount: 0,
            OverdueInvoiceCount: 0,
            OverdueAmount: 0,
            LastContactDate: DateTime.UtcNow)
        {
            FullName = email
        };
    }

    public async Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(
        GetBusinessPartnerSnapshotRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.BpCode)) return null;

        var response = await SafeGetAsync(SageX3RestEndpoints.CustomerByCode(req.BpCode), ct);
        if (response == null) return null;

        return new BusinessPartnersResponse(new BusinessPartnerSnapshotDto(
            BpCode: req.BpCode,
            Company: req.BpCode,
            CreditLimit: 0,
            TotalOutstanding: 0,
            OpenOrderCount: 0,
            OverdueInvoiceCount: 0,
            OverdueAmount: 0,
            LastContactDate: DateTime.UtcNow));
    }

    public async Task<SalesOrderDetailsResponse?> FetchSalesOrderAsync(GetSalesOrderRequest req, CancellationToken ct = default)
    {
        // OrderId maps to local DB; Sage lookup uses SOHNUM once repository provides it
        return null;
    }

    public async Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct = default)
    {
        // Sage invoices are keyed by SIHNUM (string), not Guid — use repository until mapped
        return null;
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var response = await SafeGetAsync(SageX3RestEndpoints.SalesInvoicesQuery(pageSize), ct);
        if (response == null) return [];

        return [];
    }

    public async Task<InventoryItemDto?> GetItemDetailsAsync(Guid id, CancellationToken ct = default)
        => null;

    public async Task<IReadOnlyList<InventoryItemDto>> GetWarehouseStockAsync(string wh, CancellationToken ct = default)
    {
        var response = await SafeGetAsync(SageX3RestEndpoints.StockBySite(wh), ct);
        if (response == null) return [];

        return [];
    }

    public async Task<int> GetLowStockCountAsync(CancellationToken ct = default) => 0;

    public async Task<int> GetActivePartnersCountAsync(CancellationToken ct = default)
    {
        var response = await SafeGetAsync(SageX3RestEndpoints.CustomerQuery(1), ct);
        return response != null ? 1 : 0;
    }

    public async Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default) => 0;
    public async Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default) => 0;
    public async Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default) => 0;
    public async Task<int> GetHighRiskCreditAccountsCountAsync(CancellationToken ct = default) => 0;

    public async Task<decimal> SumGlobalOutstandingReceivablesAsync(CancellationToken ct = default) => 0m;
    public async Task<decimal> SumGlobalMonthlySalesAsync(CancellationToken ct = default) => 0m;
    public async Task<decimal> SumUserOutstandingReceivablesAsync(string userId, CancellationToken ct = default) => 0m;
    public async Task<decimal> SumUserMonthlySalesAsync(string userId, CancellationToken ct = default) => 0m;

    public Task<StockAvailabilityResponse> VerifyStockLevelsAsync(CheckStockRequest r, CancellationToken ct = default)
        => Task.FromResult(new StockAvailabilityResponse(false, 0));

    public Task<AdjustStockResponse> PostStockAdjustmentAsync(StockAdjustmentRequest r, CancellationToken ct = default)
        => Task.FromResult(new AdjustStockResponse(false, "Sage X3 not connected"));

    public Task<StockAdjustmentDto> FetchAdjustmentLogAsync(StockAdjustmentRequest r, CancellationToken ct = default)
        => Task.FromResult(new StockAdjustmentDto());

    public Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct = default)
        => Task.FromResult(new UpdateCreditLimitResponse(false));

    public Task<CreateClientFromEmailResponse> ProvisionPartnerAccountAsync(CreateClientFromEmailRequest request, CancellationToken ct = default)
        => Task.FromResult(new CreateClientFromEmailResponse());

    public Task<CreateSalesOrderResponse> SubmitSalesOrderAsync(CreateSalesOrderRequest req, CancellationToken ct = default)
        => Task.FromResult(new CreateSalesOrderResponse(Guid.Empty));

    public Task<InvoiceDto> PostInvoiceAsync(InvoiceDto dto, CancellationToken ct = default)
        => Task.FromResult(dto);

    public Task<InvoiceDto> GenerateInvoiceFromSalesOrderAsync(Guid orderId, CancellationToken ct = default)
        => Task.FromResult(new InvoiceDto());
}
