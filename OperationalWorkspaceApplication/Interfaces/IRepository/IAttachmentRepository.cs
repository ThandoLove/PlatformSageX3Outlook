using System.Net.Mail;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository
{

    public interface IAttachmentRepository
    {
        Task<IReadOnlyList<OperationalWorkspace.Domain.Entities.Attachment>> GetByOwnerAsync(
            string ownerType,
            string ownerId,
            CancellationToken ct);

        Task<OperationalWorkspace.Domain.Entities.Attachment?> GetAsync(Guid id, CancellationToken ct);

        System.Threading.Tasks.Task AddAsync(OperationalWorkspace.Domain.Entities.Attachment attachment, CancellationToken ct);

        void Remove(OperationalWorkspace.Domain.Entities.Attachment attachment);

        // ADD THIS: Fetch recent items (e.g., last 5 or 10) for a specific user
        Task<List<OperationalWorkspace.Domain.Entities.Attachment>> GetRecentByUserIdAsync(
            string userId,
            CancellationToken ct = default);
    }

}
