using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs;


public sealed class StockAdjustmentDto
{
    // ===== Identification =====
    public Guid Id { get; init; }
    public string AdjustmentNumber { get; init; } = string.Empty;

    // ===== Item =====
    public Guid InventoryItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemDescription { get; init; } = string.Empty;

    // ===== Warehouse Context =====
    public string WarehouseCode { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;

    // ===== Adjustment Details =====
    public decimal QuantityBefore { get; init; }
    public decimal QuantityAdjusted { get; init; } // + or -
    public decimal QuantityAfter { get; init; }

    public string AdjustmentType { get; init; } = string.Empty;
    // Increase | Decrease | Damage | WriteOff | Correction | StockCountVariance

    public string ReasonCode { get; init; } = string.Empty;
    public string? Notes { get; init; }

    // ===== Financial Impact =====
    public decimal UnitCost { get; init; }
    public decimal AdjustmentValue { get; init; } // QuantityAdjusted * UnitCost
    public string CurrencyCode { get; init; } = "USD";

    // ===== Batch / Serial (Optional) =====
    public string? BatchNumber { get; init; }
    public string? SerialNumber { get; init; }

    // ===== Approval & Status =====
    public string Status { get; init; } = string.Empty;
    // Draft | PendingApproval | Approved | Rejected | Posted

    public bool RequiresApproval { get; init; }
    public bool IsPostedToLedger { get; init; }

    // ===== ERP Sync =====
    public string? SageAdjustmentNumber { get; init; }
    public bool IsSyncedToErp { get; init; }
    public DateTime? SyncedAtUtc { get; init; }

    // ===== Audit =====
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid? ApprovedBy { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
    public Guid AdjustmentId { get; internal set; }
    public decimal QuantityChange { get; internal set; }
    public object AdjustedBy { get; internal set; } = new object();
    public DateTime AdjustedAtUtc { get; internal set; }
}