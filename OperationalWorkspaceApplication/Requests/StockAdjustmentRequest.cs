using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests;


public sealed class StockAdjustmentRequest
{
    // ===== Target Item =====
    public Guid InventoryItemId { get; init; }

    // ===== Warehouse Context =====
    public string WarehouseCode { get; init; } = string.Empty;

    // ===== Adjustment Details =====
    public decimal QuantityChange { get; init; }
    // Positive = Increase
    // Negative = Decrease

    public string AdjustmentType { get; init; } = string.Empty;
    // Increase | Decrease | Damage | WriteOff | Correction | StockCountVariance

    public string ReasonCode { get; init; } = string.Empty;
    public string? Notes { get; init; }

    // ===== Cost Override (Optional) =====
    public decimal? OverrideUnitCost { get; init; }

    // ===== Batch / Serial Tracking =====
    public string? BatchNumber { get; init; }
    public string? SerialNumber { get; init; }

    // ===== Approval Context =====
    public bool ForceApproval { get; init; }

    // ===== User Context =====
    public Guid PerformedByUserId { get; init; }

    // ===== Correlation / Integration =====
    public string? CorrelationId { get; init; }
    public string SourceSystem { get; init; } = "API";
}