namespace OperationalWorkspace.Domain.Entities;

public class BusinessPartner
{
    public int Id { get; private set; }
    public string BpCode { get; private set; } = default!;
    public string Company { get; private set; } = default!;
    public decimal CreditLimit { get; private set; }
    public decimal OverdueInvoices { get; private set; }
    public DateTime LastContactDate { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public BusinessPartner(string bpCode, string company, decimal limit)
    {
        BpCode = bpCode;
        Company = company;
        CreditLimit = limit;
    }

    public void UpdateCreditLimit(decimal newLimit)
    {
        if (newLimit < 0) throw new ArgumentException("Limit cannot be negative");
        CreditLimit = newLimit;
        LastContactDate = DateTime.UtcNow;
    }

    public bool IsOverCreditLimit(decimal amount) => (OverdueInvoices + amount) > CreditLimit;
}
