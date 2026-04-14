namespace OperationalWorkspaceApplication.DTOs;

// 1. Add this NEW class above your ClientDto in the same file
public class CustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
}

// 2. MODIFY your existing ClientDto to add ": CustomerDto"
public class ClientDto : CustomerDto
{
    // REMOVE CustomerId, Email, and CreditLimit from here 
    // because they are now "inherited" from the class above.

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public decimal Balance { get; set; }
}
