using OperationalWorkspace.Domain.Enums;
using OperationalWorkspace.Domain.ValueObjects;

namespace OperationalWorkspace.Domain.Entities;

public class SalesOrder
{
    public string OrderId { get; private set; } = Guid.NewGuid().ToString();
    public string BpCode { get; private set; } = default!;

    public bool IsClosed => Status == SalesOrderStatus.Completed || Status == SalesOrderStatus.Cancelled;

    // ADD THIS: EF Core requires a parameterless constructor
    private SalesOrder() { }

    public int OrderNumber { get; init; } = new Random().Next(1000, 9999);
    public SalesOrderStatus Status { get; private set; } = SalesOrderStatus.Draft;
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    private readonly List<SalesOrderLine> _lines = new();
    public IReadOnlyCollection<SalesOrderLine> Lines => _lines.AsReadOnly();

    public SalesOrder(string bpCode, List<SalesOrderLine> lines) => BpCode = bpCode;

    public void AddLine(string itemCode, int quantity, Money unitPrice)
    {
        if (Status != SalesOrderStatus.Draft) throw new InvalidOperationException("Cannot modify confirmed order.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        _lines.Add(new SalesOrderLine(itemCode, quantity, unitPrice));
    }

    public decimal TotalAmount => _lines.Sum(l => l.LineTotal);

    public Guid Id { get; set; }
    public Guid BusinessPartnerId { get; set; }

    public void Confirm() => Status = SalesOrderStatus.Confirmed;
}

public record SalesOrderLine(string ItemCode, int Quantity, Money UnitPrice)
{
    public decimal LineTotal => Quantity * UnitPrice.Amount;
}
