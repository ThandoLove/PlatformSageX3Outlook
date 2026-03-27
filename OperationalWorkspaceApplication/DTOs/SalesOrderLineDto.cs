namespace OperationalWorkspaceApplication.DTOs;

public sealed class SalesOrderLineDto
{
    public Guid Id { get; init; }

    public Guid ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemDescription { get; init; } = string.Empty;

    public decimal QuantityOrdered { get; init; }
    public decimal QuantityDelivered { get; init; }

    public decimal UnitPrice { get; init; }
    public decimal DiscountPercent { get; init; }
    public decimal TaxPercent { get; init; }

    public decimal LineSubTotal { get; init; }
    public decimal LineTaxAmount { get; init; }
    public decimal LineTotal { get; init; }

    public string LineStatus { get; init; } = string.Empty;

    public bool HasStockIssue { get; init; }
}