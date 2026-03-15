using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Mappers;

public static class ApplicationMapper
{
    public static AttachmentDto ToAttachmentDto(Attachment attachment) =>
        new AttachmentDto(
            attachment.Id,
            attachment.FileName,
            attachment.ContentType,
            attachment.FileSize,
            attachment.CreatedAt); // This must match the property in your Entity
}

