using Microsoft.AspNetCore.Hosting;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Attachments
{
    public class MockAttachmentProvider : IAttachmentProvider
    {
        private readonly IWebHostEnvironment _env;

        public MockAttachmentProvider(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<List<AttachmentDto>> GetAttachmentsAsync(
            string ownerType,
            string ownerId,
            CancellationToken cancellationToken)
        {
            var folder = Path.Combine(_env.WebRootPath, "mock-documents");

            // Absolute safety check: If folder doesn't exist, create it to prevent crashes
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var files = Directory.GetFiles(folder);

            var docs = files.Select(file =>
            {
                var info = new FileInfo(file);

                // 🚀 FIXED: Passes parameters as positional record arguments matching your declared constructor sequence exactly!
                return new AttachmentDto(
                    Guid.NewGuid(),
                    info.Name,
                    GetContentType(info.Extension),
                    info.Length,
                    $"/mock-documents/{info.Name}", // Maps straight to FileUrl
                    "SageX3-GlobalRepo",             // Maps straight to RelatedEntity
                    info.LastWriteTimeUtc            // Maps straight to UploadedAtUtc
                );
            }).ToList();

            return Task.FromResult(docs);
        }

        private string GetContentType(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return "application/octet-stream";

            return extension.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }
    }
}
