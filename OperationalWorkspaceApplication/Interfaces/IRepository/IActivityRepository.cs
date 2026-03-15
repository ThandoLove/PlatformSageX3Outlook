using OperationalWorkspace.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceApplication.Interfaces.IRepository

{ 
public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Activity>> GetByRelatedEntityAsync(Guid entityId, CancellationToken ct = default);
    Task AddAsync(Activity activity, CancellationToken ct = default);
}
}