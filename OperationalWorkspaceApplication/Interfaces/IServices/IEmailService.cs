using OperationalWorkspaceApplication.DTOs;
using System.Threading.Tasks;


public interface IEmailService
{
    Task<bool> SyncEmailAsync(EmailInsightDto dto);
    Task<EmailInsightDto?> GetEmailByIdAsync(string emailId);
    Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId);
    Task<List<TaskDto>> GetLinkedTasksAsync(string emailId);
}