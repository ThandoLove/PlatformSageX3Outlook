
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces
{
    public interface IAttachmentProvider
    {
        Task<List<AttachmentDto>>
        GetAttachmentsAsync(
            string ownerType,
            string ownerId,
            CancellationToken cancellationToken);
    }
}