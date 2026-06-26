using OperationalWorkspace.Domain.Enums;
using OperationalWorkspace.Domain.ValueObjects;

namespace OperationalWorkspace.Domain.Entities;

public class SalesOrder
{
    // Properties - Kept completely intact for EF Core and read-only query mapping
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string OrderId { get; private set; } = Guid.NewGuid().ToString();
    public string BpCode { get; private set; } = default!;
    public Guid BusinessPartnerId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    public int OrderNumber { get; init; } = new Random().Next(1000, 9999);
    public SalesOrderStatus Status { get; private set; } = SalesOrderStatus.Draft;

    private readonly List<SalesOrderLine> _lines = new();
    public IReadOnlyCollection<SalesOrderLine> Lines => _lines.AsReadOnly();

    public bool IsClosed => Status == SalesOrderStatus.Completed || Status == SalesOrderStatus.Cancelled;
    public decimal TotalAmount => _lines.Sum(l => l.LineTotal);

    // Only EF Core constructor is allowed.
    // This allows data to be read and hydrated from the database/API, 
    // but makes it impossible to instantiate a new order anywhere else in the code.
    private SalesOrder() { }
}

/// <summary>
/// Represents an individual line item in a sales order.
/// Kept read-only to preserve reference data.
/// </summary>
public record SalesOrderLine(string ItemCode, decimal Quantity, Money UnitPrice)
{
    public decimal LineTotal => Quantity * UnitPrice.Amount;
}
