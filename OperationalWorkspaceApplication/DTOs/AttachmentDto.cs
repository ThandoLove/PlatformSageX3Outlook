using OperationalWorkspace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
    public sealed record AttachmentDto(
    Guid Id,
    string FileName,
    string ContentType,
    long FileSize,
    string FileUrl,
    string RelatedEntity,
    DateTime UploadedAtUtc);
  
  
}
