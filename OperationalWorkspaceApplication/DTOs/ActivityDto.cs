

namespace OperationalWorkspaceApplication.DTOs
{

    public record ActivityDto(Guid Id, string Title, string Description, string ActivityType, Guid? RelatedEntityId, DateTime CreatedAt, string CreatedBy);
    public record CreateActivityDto(string Title, string Description, string ActivityType, Guid? RelatedEntityId);
}