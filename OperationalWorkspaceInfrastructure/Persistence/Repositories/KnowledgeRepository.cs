using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;


namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public sealed class KnowledgeRepository : IKnowledgeRepository
    {
        private readonly IntegrationDbContext _db;
        public KnowledgeRepository(IntegrationDbContext db) => _db = db;
        public async Task<Knowledge?> GetByIdAsync(Guid id) => await _db.KnowledgeBase.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        public async Task<IEnumerable<Knowledge>> SearchAsync(string q) => await _db.KnowledgeBase.AsNoTracking().Where(x => x.Title.Contains(q) || x.Content.Contains(q)).ToListAsync();
    }
}
