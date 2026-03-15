

namespace OperationalWorkspaceApplication.DTOs;


public sealed class InventoryItemDto
{
    // ===== Core Identification =====
    public Guid Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemDescription { get; init; } = string.Empty;
    public string ItemCategory { get; init; } = string.Empty;

    // ===== Warehouse =====
    public string WarehouseCode { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;

    // ===== Stock Quantities =====
    public decimal QuantityOnHand { get; init; }
    public decimal QuantityReserved { get; init; }
    public decimal QuantityAvailable { get; init; }
    public decimal QuantityOnOrder { get; init; }

    // ===== Reorder Rules =====
    public decimal ReorderPoint { get; init; }
    public decimal ReorderQuantity { get; init; }
    public decimal MinimumStockLevel { get; init; }
    public decimal MaximumStockLevel { get; init; }

    // ===== Costing / Valuation =====
    public string CostingMethod { get; init; } = string.Empty;
    // FIFO | LIFO | WeightedAverage | Standard

    public decimal UnitCost { get; init; }
    public decimal AverageCost { get; init; }
    public decimal InventoryValue { get; init; }

    // ===== Status Flags =====
    public bool IsActive { get; init; }
    public bool IsStockItem { get; init; }
    public bool IsBackOrdered { get; init; }
    public bool IsLowStock { get; init; }
    public bool IsOverStocked { get; init; }

    // ===== Compliance / Controls =====
    public bool RequiresBatchTracking { get; init; }
    public bool RequiresSerialTracking { get; init; }
    public bool IsBlockedForSale { get; init; }

    // ===== ERP Sync =====
    public string? SageItemNumber { get; init; }
    public bool IsSyncedToErp { get; init; }
    public DateTime? LastSyncedAtUtc { get; init; }

    // ===== Audit =====
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? LastUpdatedAtUtc { get; init; }

    // ===== Computed Helpers =====
    public bool NeedsReorder =>
        QuantityAvailable <= ReorderPoint && IsActive;

    public decimal AvailableAfterReservations =>
        QuantityOnHand - QuantityReserved;

    public Guid ItemId { get; set; } // Fixed naming
    
    public string Description { get; set; } = string.Empty; // Added
 
   
    public DateTime LastUpdatedUtc { get; set; } // Added
}