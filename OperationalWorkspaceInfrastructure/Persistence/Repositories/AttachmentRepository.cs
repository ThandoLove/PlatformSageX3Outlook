using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly IntegrationDbContext _db;
    public AttachmentRepository(IntegrationDbContext db) => _db = db;

    // Matches GetByOwnerAsync
    public async Task<IReadOnlyList<Attachment>> GetByOwnerAsync(string ownerType, string ownerId, CancellationToken ct)
    {
        return await _db.Attachments
            .Where(a => a.OwnerType == ownerType && a.OwnerId == ownerId)
            .ToListAsync(ct);
    }

    // Matches GetAsync
    public async Task<Attachment?> GetAsync(Guid id, CancellationToken ct)
        => await _db.Attachments.FindAsync(new object[] { id }, ct);

    // Matches AddAsync
    public async Task AddAsync(Attachment attachment, CancellationToken ct)
    {
        await _db.Attachments.AddAsync(attachment, ct);
        // Note: SaveChangesAsync is usually called in the UnitOfWork
    }

    // Matches Remove
    public void Remove(Attachment attachment)
    {
        _db.Attachments.Remove(attachment);
    }
}
