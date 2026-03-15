using OperationalWorkspace.Domain.Enums;

namespace OperationalWorkspace.Domain.Entities;

public class Invoice
{
    // FIX: Change init to set so EF and Repository can assign values
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BusinessPartnerId { get; set; }

    // FIX: Changed to string to match the Repository logic (INV-DATE-ID)
    public string InvoiceNumber { get; set; } = string.Empty;

    public string InvoiceId { get; set; } = string.Empty;
    public string BpCode { get; set; } = string.Empty;

    // FIX: Change private set to set so Repository can map these from DB/Orders
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public decimal OutstandingAmount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FIX: Added parameterless constructor for EF Core and Repository Initialization
    public Invoice() { }

    public Invoice(string bpCode, decimal amount)
    {
        BpCode = bpCode;
        Amount = amount;
    }

    public void ProcessPayment(decimal payment)
    {
        if (Status == InvoiceStatus.Void) throw new InvalidOperationException("Invoice is void.");
        AmountPaid += payment;
        Status = AmountPaid >= Amount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
    }
}
