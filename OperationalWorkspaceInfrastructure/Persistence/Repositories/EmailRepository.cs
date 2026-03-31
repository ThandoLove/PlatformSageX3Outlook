using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using System.Threading.Tasks; // DO NOT ALIAS THIS

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public sealed class EmailRepository : IEmailRepository
    {
        private readonly IntegrationDbContext _db;

        public EmailRepository(IntegrationDbContext db)
        {
            _db = db;
        }

        public async System.Threading.Tasks.Task<bool> ExistsAsync(string mid)
        {
            return await _db.Emails.AnyAsync(x => x.MessageId == mid);
        }

        public async System.Threading.Tasks.Task AddAsync(Email e)
        {
            await _db.Emails.AddAsync(e);
            await _db.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<Email?> GetByMessageIdAsync(string messageId)
        {
            return await _db.Emails
                .FirstOrDefaultAsync(e => e.MessageId == messageId);
        }
    }
}
