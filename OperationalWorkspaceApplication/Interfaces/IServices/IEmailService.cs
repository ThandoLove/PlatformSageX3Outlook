using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IEmailService
    {
        // 1. STORE EMAIL FROM OUTLOOK ENVELOPE
        Task<bool> SyncEmailAsync(EmailInsightDto dto);

        // 2. GET BASIC EMAIL DATA
        Task<EmailInsightDto?> GetEmailByIdAsync(Guid emailId);

        // 3. ⭐ REAL INTELLIGENCE WORKSPACE ENTRY POINT
        // Returns the full composite dashboard context validated via strict Guid keys
        Task<EmailContextDto?> GetEmailContextAsync(Guid emailId);

        // 4. ARCHITECTURAL FORWARDERS
        Task<List<OpenOrderDto>> GetLinkedOrdersAsync(Guid emailId);
        Task<List<TaskDto>> GetLinkedTasksAsync(Guid emailId);
    }
}
