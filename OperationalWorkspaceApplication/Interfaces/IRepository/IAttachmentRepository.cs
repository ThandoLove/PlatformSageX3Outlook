using System.Net.Mail;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository
{

    public interface IAttachmentRepository
    {
        // Use the full namespace here to force the correct type
        Task<IReadOnlyList<OperationalWorkspace.Domain.Entities.Attachment>> GetByOwnerAsync(
            string ownerType,
            string ownerId,
            CancellationToken ct);

        Task<OperationalWorkspace.Domain.Entities.Attachment?> GetAsync(Guid id, CancellationToken ct);

        System.Threading.Tasks.Task AddAsync(OperationalWorkspace.Domain.Entities.Attachment attachment, CancellationToken ct);

        void Remove(OperationalWorkspace.Domain.Entities.Attachment attachment);
    }

}
