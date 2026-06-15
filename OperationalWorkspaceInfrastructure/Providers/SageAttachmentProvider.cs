using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;

namespace OperationalWorkspaceInfrastructure.Attachments;

public class SageAttachmentProvider : IAttachmentProvider
{
    public async Task<List<AttachmentDto>> GetAttachmentsAsync(
        string ownerType,
        string ownerId,
        CancellationToken cancellationToken)
    {
        return new List<AttachmentDto>();

        // Later:
        // Call Sage X3 Web Services
        // Map Sage attachments into AttachmentDto
    }
}