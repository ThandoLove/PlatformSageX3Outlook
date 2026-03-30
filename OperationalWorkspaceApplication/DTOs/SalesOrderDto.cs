using OperationalWorkspaceApplication.DTOs;

public sealed class SalesOrderDto
{
    public Guid Id { get; init; }

    public string OrderNumber { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public DateTime? RequestedDeliveryDate { get; init; }

    public Guid BusinessPartnerId { get; init; }
    public string BusinessPartnerCode { get; init; } = string.Empty;
    public string BusinessPartnerName { get; init; } = string.Empty;

    public string CurrencyCode { get; init; } = "USD";
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }

    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }

    public string OrderStatus { get; init; } = string.Empty;
    public string FulfillmentStatus { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;

    public string WarehouseCode { get; init; } = string.Empty;
    public string ShippingMethod { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;

    public IReadOnlyCollection<SalesOrderLineDto> Lines { get; init; }
        = Array.Empty<SalesOrderLineDto>();

    

    public Guid CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid? LastModifiedBy { get; init; }
    public DateTime? LastModifiedAtUtc { get; init; }

    public string? SageDocumentNumber { get; init; }
    public bool IsSyncedToErp { get; init; }

    public bool IsOverdue =>
        RequestedDeliveryDate.HasValue &&
        RequestedDeliveryDate.Value.Date < DateTime.UtcNow.Date &&
        FulfillmentStatus != "Completed";
}