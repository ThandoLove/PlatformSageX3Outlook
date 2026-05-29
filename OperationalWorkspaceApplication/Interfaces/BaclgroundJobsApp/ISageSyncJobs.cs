
using OperationalWorkspaceApplication.DTOs;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.BackgroundJobsApp;

public interface ISageSyncJobs
{
    Task ExecuteSyncAsync(string orderId, string userId);

    Task EnqueueContactCreationAsync(ContactCreateDto dto);
}
