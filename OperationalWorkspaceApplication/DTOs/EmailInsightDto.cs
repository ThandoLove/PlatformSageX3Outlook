

namespace OperationalWorkspaceApplication.DTOs;

public sealed class EmailInsightDto
{
    // Core Identification
    public Guid BusinessPartnerId { get; init; }
    public string BusinessPartnerCode { get; init; } = string.Empty;
    public string BusinessPartnerName { get; init; } = string.Empty;

    // Financial Snapshot
    public decimal TotalOutstanding { get; init; }
    public decimal CurrentAmount { get; init; }
    public decimal Overdue30 { get; init; }
    public decimal Overdue60 { get; init; }
    public decimal Overdue90 { get; init; }
    public decimal Overdue120Plus { get; init; }

    public bool HasOverdueInvoices { get; init; }
    public int OpenInvoiceCount { get; init; }

    // Order Snapshot
    public int OpenOrderCount { get; init; }
    public decimal OpenOrderValue { get; init; }

    // Task Snapshot
    public int OpenTaskCount { get; init; }
    public int AssignedTaskCount { get; init; }

    // Inventory Alerts
    public bool HasBackOrders { get; init; }
    public bool HasLowStockImpact { get; init; }

    // Risk & Status Flags
    public bool IsOnCreditHold { get; init; }
    public string RiskLevel { get; init; } = "Normal";
    public string AccountStatus { get; init; } = "Active";

    // ERP Metadata
    public DateTime SnapshotGeneratedAtUtc { get; init; }


    //email metadata

    public string MessageId { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string From { get; init; } = string.Empty;   
    public DateTime ReceivedAt { get; init; }
    public string AssignedUserId { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
}