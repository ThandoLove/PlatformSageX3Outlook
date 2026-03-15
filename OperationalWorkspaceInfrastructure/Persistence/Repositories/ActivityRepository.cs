using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;


namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public sealed class ActivityRepository : IActivityRepository
    {
        private readonly IntegrationDbContext _db;
        public ActivityRepository(IntegrationDbContext db) => _db = db;
        public async Task AddAsync(Activity a, CancellationToken ct) { await _db.Activities.AddAsync(a, ct); await _db.SaveChangesAsync(ct); }
        public async Task<Activity?> GetByIdAsync(Guid id, CancellationToken ct) => await _db.Activities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        public async Task<IEnumerable<Activity>> GetByRelatedEntityAsync(Guid rid, CancellationToken ct) => await _db.Activities.AsNoTracking().Where(x => x.RelatedEntityId == rid).ToListAsync(ct);
    }
}
