using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface IInventoryService
{
    // COMMANDS
    Task<AdjustStockResponse> AdjustStockAsync(
        StockAdjustmentRequest request,
        CancellationToken cancellationToken);

    // QUERIES
    Task<StockAvailabilityResponse> CheckAvailabilityAsync(
        CheckStockRequest request,
        CancellationToken cancellationToken);

    Task<InventoryItemDto?> GetItemAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<InventoryItemDto>> GetWarehouseInventoryAsync(
        string warehouseCode,
        CancellationToken cancellationToken);

    // Additional DTO-specific logic
    Task<StockAdjustmentDto> GetAdjustmentDetailsAsync(
        StockAdjustmentRequest request,
        CancellationToken ct);
    Task<int> CountStockAlertsAsync();
}
