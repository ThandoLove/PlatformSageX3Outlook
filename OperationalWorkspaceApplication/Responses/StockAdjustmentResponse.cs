

namespace OperationalWorkspaceApplication.Responses;

public class StockAdjustmentResponse
{
    public Guid AdjustmentId { get; set; } = Guid.NewGuid();
    public string ItemCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;

    // The quantity that was added or removed
    public decimal AdjustedQuantity { get; set; }

    // The new balance after the adjustment
    public decimal NewQuantity { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "Stock adjustment processed successfully.";
}
