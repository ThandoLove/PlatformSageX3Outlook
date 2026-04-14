namespace OperationalWorkspaceApplication.DTOs;

public class CustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
}

public class ClientDto : CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public decimal Balance { get; set; }

    // --- ADD THESE BACK TO FIX THE UI ERRORS ---
    public string Category { get; set; } = string.Empty; // Fixed: Category error
    public string Currency { get; set; } = "USD";        // Fixed: Currency error
    public decimal TotalRevenue { get; set; }           // Fixed: TotalRevenue error
}
