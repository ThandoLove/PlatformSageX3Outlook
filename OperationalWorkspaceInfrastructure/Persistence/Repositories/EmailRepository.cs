using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;


namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public sealed class EmailRepository : IEmailRepository
    {
        private readonly IntegrationDbContext _db;
        public EmailRepository(IntegrationDbContext db) => _db = db;
        public async Task<bool> ExistsAsync(string mid) => await _db.Emails.AnyAsync(x => x.MessageId == mid);
        public async Task AddAsync(Email e) { await _db.Emails.AddAsync(e); await _db.SaveChangesAsync(); }
    }
}
