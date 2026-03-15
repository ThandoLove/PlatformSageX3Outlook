using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs;


public sealed class SalesOrderDto
{
    private object id;
    private string bpCode;

    public SalesOrderDto(object id, string bpCode, decimal totalAmount)
    {
        this.id = id;
        this.bpCode = bpCode;
        TotalAmount = totalAmount;
    }

    // ===== Header =====
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public DateTime? RequestedDeliveryDate { get; init; }

    // ===== Business Partner =====
    public Guid BusinessPartnerId { get; init; }
    public string BusinessPartnerCode { get; init; } = string.Empty;
    public string BusinessPartnerName { get; init; } = string.Empty;

    // ===== Financials =====
    public string CurrencyCode { get; init; } = "USD";
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }

    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }

    // ===== Status =====
    public string OrderStatus { get; init; } = string.Empty;
    // Draft | Confirmed | PartiallyDelivered | Delivered | Cancelled

    public string FulfillmentStatus { get; init; } = string.Empty;
    // NotStarted | Picking | Shipped | Completed

    public string PaymentStatus { get; init; } = string.Empty;
    // Unpaid | PartiallyPaid | Paid | Overdue

    // ===== Logistics =====
    public string WarehouseCode { get; init; } = string.Empty;
    public string ShippingMethod { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;

    // ===== Lines =====
    public IReadOnlyCollection<SalesOrderLineDto> Lines { get; init; }
        = Array.Empty<SalesOrderLineDto>();

    // ===== Audit =====
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid? LastModifiedBy { get; init; }
    public DateTime? LastModifiedAtUtc { get; init; }

    // ===== ERP Integration =====
    public string? SageDocumentNumber { get; init; }
    public bool IsSyncedToErp { get; init; }

    // ===== Computed Flags =====
    public bool IsOverdue =>
        RequestedDeliveryDate.HasValue &&
        RequestedDeliveryDate.Value.Date < DateTime.UtcNow.Date &&
        FulfillmentStatus != "Completed";
}