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

        // Hardened lookup accepts the Outlook message id string
        public async System.Threading.Tasks.Task<Email?> GetByMessageIdAsync(string outlookMessageId)
        {
            if (string.IsNullOrWhiteSpace(outlookMessageId)) return null;

            return await _db.Emails
                .FirstOrDefaultAsync(e => e.MessageId == outlookMessageId);
        }
    }
}
