
namespace OperationalWorkspace.Domain.Entities;

public sealed class Activity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActivityType { get; set; } = "Note"; // e.g., Call, Email, Meeting
    public Guid? RelatedEntityId { get; set; } // Link to SalesOrder, Partner, etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty; // Outlook User Email
   

}
