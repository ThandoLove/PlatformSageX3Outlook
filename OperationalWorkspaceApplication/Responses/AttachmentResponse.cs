using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceApplication.Responses
{
    public sealed record UploadAttachmentResponse(Guid AttachmentId);

    public sealed record AttachmentListResponse(
        IReadOnlyList<AttachmentDto> Attachments);
}
