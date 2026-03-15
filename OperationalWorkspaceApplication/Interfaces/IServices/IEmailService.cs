using OperationalWorkspaceApplication.DTOs;
using System.Threading.Tasks;


public interface IEmailService
{
    Task<bool> SyncEmailAsync(EmailInsightDto dto);
}