using System;

namespace OperationalWorkspace.Domain.Entities;

public class BusinessPartner
{
    // Primary Key
    public int Id { get; private set; }

    // Core Properties
    public string BpCode { get; private set; } = default!;
    public string Company { get; private set; } = default!;
    public decimal CreditLimit { get; private set; }
    public decimal OverdueInvoices { get; private set; }
    public DateTime LastContactDate { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // ===== NEW PROPERTIES FOR REPOSITORY & DASHBOARD =====
    public bool IsLead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool HasOpenOpportunity { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? LastInteractionNote { get; set; }
    public DateTime? LastInteractionDate { get; set; }
    public decimal TotalSalesVolume { get; set; }
    // ======================================================

    // Existing Constructor
    public BusinessPartner(string bpCode, string company, decimal limit)
    {
        BpCode = bpCode;
        Company = company;
        CreditLimit = limit;
        CreatedAt = DateTime.UtcNow; // Initialize CreatedAt
    }

    // Required for EF Core
    private BusinessPartner() { }

    // Existing Methods
    public void UpdateCreditLimit(decimal newLimit)
    {
        if (newLimit < 0) throw new ArgumentException("Limit cannot be negative");
        CreditLimit = newLimit;
        LastContactDate = DateTime.UtcNow;
    }

    public bool IsOverCreditLimit(decimal amount) => (OverdueInvoices + amount) > CreditLimit;
}
