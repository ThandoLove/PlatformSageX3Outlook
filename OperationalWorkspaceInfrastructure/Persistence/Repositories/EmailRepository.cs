using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using System;
using System.Threading.Tasks; // DO NOT ALIAS THIS

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public sealed class EmailRepository : IEmailRepository
    {
        private readonly IntegrationDbContext _db;

        public EmailRepository(IntegrationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async System.Threading.Tasks.Task<bool> ExistsAsync(string mid)
        {
            if (string.IsNullOrWhiteSpace(mid)) return false;

            return await _db.Emails.AnyAsync(x => x.MessageId == mid);
        }

        public async System.Threading.Tasks.Task AddAsync(Email e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            await _db.Emails.AddAsync(e);
            await _db.SaveChangesAsync();
        }

        // Hardened lookup accepts the type-safe Guid tracking parameter
        public async System.Threading.Tasks.Task<Email?> GetByMessageIdAsync(Guid emailId)
        {
            if (emailId == Guid.Empty) return null;

            // Safely converts the cryptographic token key into a clean lookup string
            var targetStringId = emailId.ToString();

            return await _db.Emails
                .FirstOrDefaultAsync(e => e.MessageId == targetStringId);
        }
    }
}
