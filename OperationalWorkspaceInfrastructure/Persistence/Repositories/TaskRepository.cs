using Microsoft.EntityFrameworkCore; // Required for FirstOrDefaultAsync/ToListAsync
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceInfrastructure.Persistence;
using Entities = OperationalWorkspace.Domain.Entities; // Alias for your entity
using Task = System.Threading.Tasks.Task; // Alias for async keyword

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IntegrationDbContext _db;
    public TaskRepository(IntegrationDbContext db) => _db = db;

    // Interface Implementation: AddAsync
    public async Task AddAsync(Entities.Task task, CancellationToken ct)
    {
        await _db.Tasks.AddAsync(task, ct);
        await _db.SaveChangesAsync(ct);
    }

    // Interface Implementation: GetByIdAsync
    public async Task<Entities.Task?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    // Interface Implementation: UpdateAsync
    public async Task UpdateAsync(Entities.Task task, CancellationToken ct)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync(ct);
    }

    // Interface Implementation: GetByUserAsync
    // Note: If your DB stores userId as a Guid, use Guid.Parse(userId)
    public async Task<IReadOnlyList<Entities.Task>> GetByUserAsync(string userId, CancellationToken ct)
    {
        return await _db.Tasks
            .Where(t => t.AssignedToUserId.ToString() == userId)
            .ToListAsync(ct);
    }
}
