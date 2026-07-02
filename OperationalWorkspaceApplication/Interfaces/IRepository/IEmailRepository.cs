using System.Threading.Tasks;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository
{
    public interface IEmailRepository
    {
        // 1. CHECKS IF EMAIL EXISTS IN DATABASE
        Task<bool> ExistsAsync(string messageId);

        // 2. ADDS NEW EMAIL RECORD (Correct Entity Binding)
        Task AddAsync(Email email);

        // 3. RETRIEVES EMAIL BY OUTLOOK MESSAGE ID (string)
        Task<Email?> GetByMessageIdAsync(string outlookMessageId);
    }
}
