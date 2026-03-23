using OperationalWorkspace.Domain.Enums;

namespace OperationalWorkspace.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BusinessPartnerId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
    public string BpCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public decimal OutstandingAmount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FIX: Use null! to satisfy the compiler's nullability check
    public string UserId { get; set; } = null!;

    // FIX: Ensure parameterless constructor exists for EF Core
    public Invoice() { }

    public Invoice(string bpCode, decimal amount, string userId)
    {
        BpCode = bpCode;
        Amount = amount;
        UserId = userId; // FIX: Assign UserId here
    }

    public void ProcessPayment(decimal payment)
    {
        if (Status == InvoiceStatus.Void) throw new InvalidOperationException("Invoice is void.");
        AmountPaid += payment;
        Status = AmountPaid >= Amount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
    }
}
