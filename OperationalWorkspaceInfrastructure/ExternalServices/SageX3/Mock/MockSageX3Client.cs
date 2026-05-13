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
    // 🟢 MOCK INVENTORY SERVICE MEMBERS
    // =========================================================================

    public Task<int> GetLowStockCountAsync(CancellationToken ct = default)
        => Task.FromResult(8);

    public Task<InventoryItemDto?> GetItemDetailsAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult<InventoryItemDto?>(new InventoryItemDto
        {
            Id = id,
            ItemCode = "MOCK-SKU",
            ItemDescription = "Mock Item",
            Description = "Mock Item"
        });

    public Task<IReadOnlyList<InventoryItemDto>> GetWarehouseStockAsync(string wh, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<InventoryItemDto>>(new List<InventoryItemDto>
        {
            new() { ItemCode = "MOCK-WH-SKU", WarehouseCode = wh }
        });

    public Task<StockAvailabilityResponse> VerifyStockLevelsAsync(CheckStockRequest r, CancellationToken ct = default)
        => Task.FromResult(new StockAvailabilityResponse(true, 10));

    public Task<AdjustStockResponse> PostStockAdjustmentAsync(StockAdjustmentRequest r, CancellationToken ct = default)
        => Task.FromResult(new AdjustStockResponse(true, "Mock Adjustment Success"));

    public Task<StockAdjustmentDto> FetchAdjustmentLogAsync(StockAdjustmentRequest r, CancellationToken ct = default)
        => Task.FromResult(new StockAdjustmentDto());

    // =========================================================================
    // 🟢 MOCK BUSINESS PARTNER MEMBERS
    // =========================================================================

    public Task<int> GetActivePartnersCountAsync(CancellationToken ct = default)
        => Task.FromResult(450);

    public Task<BusinessPartnersResponse?> GetPartnerFinancialSnapshotAsync(GetBusinessPartnerSnapshotRequest req, CancellationToken ct = default)
        => Task.FromResult<BusinessPartnersResponse?>(new BusinessPartnersResponse(new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 500m, 5000m, 2, 1, 15000m, DateTime.UtcNow)));

    public Task<UpdateCreditLimitResponse> PushCreditLimitUpdateAsync(UpdateCreditLimitRequest req, CancellationToken ct = default)
        => Task.FromResult(new UpdateCreditLimitResponse(true));

    public Task<BusinessPartnerSnapshotDto?> FindPartnerByEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult<BusinessPartnerSnapshotDto?>(new BusinessPartnerSnapshotDto("C1000", "Mock Corp", 500m, 5000m, 2, 1, 15000m, DateTime.UtcNow) { FullName = "Mock User", IsLinkedToSage = true });

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
    // 🟢 MOCK INVOICE MEMBERS
    // =========================================================================

    public Task<InvoiceDto?> GetInvoiceAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult<InvoiceDto?>(new InvoiceDto { Id = id });

    public Task<IEnumerable<InvoiceDto>> GetInvoicesPagedAsync(int page, int pageSize, CancellationToken ct = default)
        => Task.FromResult<IEnumerable<InvoiceDto>>(new List<InvoiceDto> { new() { Id = Guid.NewGuid() } });

    public Task<InvoiceDto> PostInvoiceAsync(InvoiceDto dto, CancellationToken ct = default)
        => Task.FromResult(dto);

    public Task<InvoiceDto> GenerateInvoiceFromSalesOrderAsync(Guid orderId, CancellationToken ct = default)
        => Task.FromResult(new InvoiceDto { Id = Guid.NewGuid() });

    public Task<decimal> SumGlobalOutstandingReceivablesAsync(CancellationToken ct = default)
        => Task.FromResult(50000m);

    public Task<decimal> SumGlobalMonthlySalesAsync(CancellationToken ct = default)
        => Task.FromResult(120000m);

    public Task<decimal> SumUserOutstandingReceivablesAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(12000m);

    public Task<decimal> SumUserMonthlySalesAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(8500m);

    public Task<int> GetOverdueInvoicesCountAsync(CancellationToken ct = default)
        => Task.FromResult(5);

    public Task<int> GetTotalGeneratedInvoicesCountAsync(CancellationToken ct = default)
        => Task.FromResult(120);

    public Task<int> GetUserInvoicesDueCountAsync(string userId, CancellationToken ct = default)
        => Task.FromResult(3);

    public Task<int> GetHighRiskCreditAccountsCountAsync(CancellationToken ct = default)
        => Task.FromResult(2);
}
