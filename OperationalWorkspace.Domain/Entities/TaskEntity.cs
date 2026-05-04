namespace OperationalWorkspace.Domain.Entities;

public class TaskEntity
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Changed from TaskId string to Guid Id
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Added
    public string CreatedBy { get; set; } = string.Empty; // Added
    public OperationalWorkspace.Domain.Enums.TaskStatus Status { get; set; } // Added
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Added
    public Guid AssignedToUserId { get; set; }
    public string AssignedTo { get; set; }
    public DateTime UpdatedAt { get; set; }
}
