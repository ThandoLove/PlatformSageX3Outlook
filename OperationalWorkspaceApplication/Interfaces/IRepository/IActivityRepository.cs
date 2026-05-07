using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository
{
    public interface IActivityRepository
    {
        // Just the title here
        Task<IEnumerable<Activity>> GetAllAsync(CancellationToken ct = default);

        Task<Activity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Activity>> GetByRelatedEntityAsync(Guid entityId, CancellationToken ct = default);
        Task AddAsync(Activity activity, CancellationToken ct = default);
    }
}
