namespace OperationalWorkspaceApplication.Responses
{
    // Updated: Sku is now string, AvailableQuantity is decimal to match item.QuantityOnHand
    public sealed record StockAvailabilityResponse(bool IsAvailable, decimal AvailableQuantity);

    // Updated: Now accepts two arguments (bool Success, string Message)
    public sealed record AdjustStockResponse(bool Success, string Message);
}
