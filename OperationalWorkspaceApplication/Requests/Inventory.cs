using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{ 

    public sealed record CheckStockRequest(Guid InventoryItemId, decimal Quantity);

    public sealed record AdjustStockRequest(
        Guid InventoryItemId,
        decimal QuantityChange,
        string AdjustmentType,
        string ReasonCode);

}