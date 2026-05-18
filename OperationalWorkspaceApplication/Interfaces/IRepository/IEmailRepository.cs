

using OperationalWorkspace.Domain.Entities;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IEmailRepository
{
    Task<bool> ExistsAsync(string messageId);
    Task AddAsync(Email email);

    // FIX: single parameter only (NO CancellationToken)
    Task<Email?> GetByMessageIdAsync(string messageId);
}