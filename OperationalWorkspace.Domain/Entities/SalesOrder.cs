using OperationalWorkspace.Domain.Enums;
using OperationalWorkspace.Domain.ValueObjects;

namespace OperationalWorkspace.Domain.Entities;

public class SalesOrder
{
    // Properties
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

    // Constructors
    private SalesOrder() { } // EF Core Required

    public SalesOrder(string bpCode, List<SalesOrderLine> lines)
    {
        BpCode = bpCode;
        _lines = lines ?? new List<SalesOrderLine>();
        CreatedAtUtc = DateTime.UtcNow;
    }

    // Methods
    public void AddLine(string itemCode, decimal quantity, Money unitPrice)
    {
        if (Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify confirmed order.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        // This calls the automatically generated constructor of the record
        _lines.Add(new SalesOrderLine(itemCode, quantity, unitPrice));
    }

    public void Confirm() => Status = SalesOrderStatus.Confirmed;
}

/// <summary>
/// Represents an individual line item in a sales order.
/// Using a Record with a Primary Constructor automatically handles properties and the constructor.
/// </summary>
public record SalesOrderLine(string ItemCode, decimal Quantity, Money UnitPrice)
{
    // The properties ItemCode, Quantity, and UnitPrice are generated automatically.
    public decimal LineTotal => Quantity * UnitPrice.Amount;
}
