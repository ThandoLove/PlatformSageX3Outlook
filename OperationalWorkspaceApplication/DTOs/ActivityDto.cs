using OperationalWorkspace.Domain.Entities;



namespace OperationalWorkspaceApplication.DTOs
{
    // The constructor must be INSIDE the record brackets
    public record ActivityDto(Guid Id, string Title, string Description, string ActivityType, Guid RelatedEntityId, DateTime CreatedAt, string CreatedBy, DateTime Timestamp, string Action)
    {
        // Parameterless constructor for object initializer support
        public ActivityDto() : this(Guid.Empty, "", "", "", Guid.Empty, DateTime.UtcNow, "", DateTime.UtcNow, "") { }
    }

    public record CreateActivityDto(string Title, string Description, string ActivityType, Guid? RelatedEntityId);
}
