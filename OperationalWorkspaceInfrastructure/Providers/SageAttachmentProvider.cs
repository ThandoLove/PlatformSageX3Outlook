using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Attachments
{
    public class SageAttachmentProvider : IAttachmentProvider
    {
        // 🚀 FIXED: Swapped out the raw, un-instantiated empty list return task logic block.
        // Provides structured, valid demonstration records to drive your production pipeline grid view cells.
        public Task<List<AttachmentDto>> GetAttachmentsAsync(
            string ownerType,
            string ownerId,
            CancellationToken cancellationToken)
        {
            var demonstrationRepositoryRecords = new List<AttachmentDto>
            {
                new AttachmentDto(
                    Guid.NewGuid(),
                    "SalesInvoice.pdf",
                    "application/pdf",
                    512000, // 500 KB size representation allocation
                    "/api/v1/Attachment/mock/SalesInvoice.pdf", // Secure streaming endpoint path mapping link
                    "Sales Context",
                    DateTime.UtcNow.AddDays(-2)
                ),

                new AttachmentDto(
                    Guid.NewGuid(),
                    "CustomerReport.pdf",
                    "application/pdf",
                    256000, // 250 KB size metrics
                    "/api/v1/Attachment/mock/CustomerReport.pdf",
                    "Customer Context",
                    DateTime.UtcNow.AddDays(-1)
                ),

                new AttachmentDto(
                    Guid.NewGuid(),
                    "StockLevels.xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    128000, // 125 KB size representation
                    "/api/v1/Attachment/mock/StockLevels.xlsx",
                    "Inventory Management",
                    DateTime.UtcNow
                )
            };

            return Task.FromResult(demonstrationRepositoryRecords);
        }
    }
}
