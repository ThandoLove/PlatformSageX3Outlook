namespace OperationalWorkspace.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public decimal QuantityOnHand { get; private set; }
    public decimal QuantityReserved { get; private set; }

    // FIX: Add this calculated property
    public decimal AvailableQuantity => QuantityOnHand - QuantityReserved;

    public decimal UnitCost { get; private set; }
    public DateTime LastUpdatedUtc { get; private set; }

    public InventoryItem() { }

    // FIX: Add this method for the SalesService to call
    public void Reserve(decimal quantity)
    {
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Insufficient stock to reserve.");

        QuantityReserved += quantity;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public void AdjustQuantity(decimal change, string type, DateTime updatedAt)
    {
        var projected = QuantityOnHand + change;
        if (projected < 0 && type != "WriteOff")
            throw new InvalidOperationException("Adjustment would create negative inventory.");

        QuantityOnHand = projected;
        LastUpdatedUtc = updatedAt;
    }
}
