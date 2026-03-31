using OperationalWorkspace.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IEmailRepository
{
    Task<bool> ExistsAsync(string messageId);
    Task AddAsync(Email email);
  
    Task<Email?> GetByMessageIdAsync(string emailId);
}