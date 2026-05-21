using System.Collections.Generic;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IEmailService
    {
        // 1. STORE EMAIL
        Task<bool> SyncEmailAsync(EmailInsightDto dto);

        // 2. GET BASIC EMAIL
        Task<EmailInsightDto?> GetEmailByIdAsync(string emailId);

        // 3. ⭐ REAL INTELLIGENCE ENTRY POINT (Added to fulfill contract requirements)
        Task<EmailContextDto?> GetEmailContextAsync(string emailId);

        // 4. ARCHITECTURAL FORWARDERS
        Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId);
        Task<List<TaskDto>> GetLinkedTasksAsync(string emailId);
    }
}
