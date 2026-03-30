


namespace OperationalWorkspace.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }

    // BASIC INFO
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // ADDRESS
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // BUSINESS DATA
    public string ClientCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    // AUDIT FIELDS
    // These are often handled automatically in the DbContext SaveChanges
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // NAVIGATION PROPERTIES
    // Relates to your SalesOrder entity
    public virtual ICollection<SalesOrder> Orders { get; set; } = new List<SalesOrder>();
}
