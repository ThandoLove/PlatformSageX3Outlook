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
        Task<EmailInsightDto?> GetEmailByIdAsync(string outlookMessageId);

        // 3. ⭐ REAL INTELLIGENCE WORKSPACE ENTRY POINT
        // Returns the full composite dashboard context by Outlook message id
        Task<EmailContextDto?> GetEmailContextAsync(string outlookMessageId);

        // 4. ARCHITECTURAL FORWARDERS
        Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string outlookMessageId);
        Task<List<TaskDto>> GetLinkedTasksAsync(string outlookMessageId);
    }
}
