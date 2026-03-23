using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{

    public sealed record UploadAttachmentRequest(
        string OwnerType,
        string OwnerId,
        string FileName,
        string ContentType,
        long FileSize,
        string StoragePath)
    {
        public string? Source { get; internal set; }
        public Guid EntityId { get; internal set; }
    }

    public sealed record DeleteAttachmentRequest(Guid AttachmentId);

    public sealed record GetAttachmentsRequest(
        string OwnerType,
        string OwnerId);

}