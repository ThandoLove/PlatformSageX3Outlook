using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly IntegrationDbContext _db;
    public AttachmentRepository(IntegrationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Attachment>> GetByOwnerAsync(string ownerType, string ownerId, CancellationToken ct)
    {
        return await _db.Attachments
            .Where(a => a.OwnerType == ownerType && a.OwnerId == ownerId)
            .ToListAsync(ct);
    }

    public async Task<Attachment?> GetAsync(Guid id, CancellationToken ct)
        => await _db.Attachments.FindAsync(new object[] { id }, ct);

    public async Task AddAsync(Attachment attachment, CancellationToken ct)
    {
        await _db.Attachments.AddAsync(attachment, ct);
    }

    public void Remove(Attachment attachment)
    {
        _db.Attachments.Remove(attachment);
    }

    // FIX: Implementing the new interface member for the Dashboard
    public async Task<List<Attachment>> GetRecentByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _db.Attachments
            .Where(a => a.OwnerId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(10) // Limit to the most recent 10 items
            .ToListAsync(ct);
    }
}
