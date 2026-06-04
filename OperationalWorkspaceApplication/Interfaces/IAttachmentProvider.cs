using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces
{
    public interface IAttachmentProvider
    {
        Task<List<AttachmentDto>> GetAttachmentsAsync(
            string ownerType,
            string ownerId,
            CancellationToken cancellationToken);
    }
}
