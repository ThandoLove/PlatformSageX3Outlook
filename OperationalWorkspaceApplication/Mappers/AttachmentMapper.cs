using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Mappers;

public static class ApplicationMapper
{
    public static AttachmentDto ToAttachmentDto(Attachment attachment) =>
        new AttachmentDto(
            attachment.Id,                         // 1. Guid Id
            attachment.FileName,                   // 2. string FileName
            attachment.ContentType,                // 3. string ContentType
            attachment.FileSize,                   // 4. long FileSize
            attachment.Source,                     // 5. string FileUrl
            attachment.EntityId.ToString(),        // 6. string RelatedEntity (FIXED)
            attachment.CreatedAt                   // 7. DateTime UploadedAtUtc
        );
}
