using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;

// Explicit Type Alias to resolve the namespace collision with the Services.Email folder
using EmailEntity = OperationalWorkspace.Domain.Entities.Email;

namespace OperationalWorkspaceApplication.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly IEmailRepository _repo;
        private readonly EmailContextBuilder _builder;

        public EmailService(
            IEmailRepository repo,
            EmailContextBuilder builder)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        // ---------------------------------------------------------------------
        // 1. STORE EMAIL
        // ---------------------------------------------------------------------
        public async Task<bool> SyncEmailAsync(EmailInsightDto dto)
        {
            if (await _repo.ExistsAsync(dto.MessageId))
                return false;

            // Instantiated securely using the precise Type Alias to completely bypass compiler confusion
            await _repo.AddAsync(new EmailEntity
            {
                MessageId = dto.MessageId,
                Subject = dto.Subject,
                From = dto.From,
                ReceivedAt = dto.ReceivedAt
            });

            return true;
        }

        // ---------------------------------------------------------------------
        // 2. GET BASIC EMAIL (Updated parameter to matching Guid contract format)
        // ---------------------------------------------------------------------
        public async Task<EmailInsightDto?> GetEmailByIdAsync(Guid emailId)
        {
            // Maps the Guid token out to a clean structural string tracking key
            var stringId = emailId.ToString();
            var email = await _repo.GetByMessageIdAsync(emailId);

            if (email == null)
                return null;

            return new EmailInsightDto
            {
                MessageId = email.MessageId,
                Subject = email.Subject,
                From = email.From,
                ReceivedAt = email.ReceivedAt
            };
        }

        // ---------------------------------------------------------------------
        // 3. ⭐ REAL INTELLIGENCE ENTRY POINT (Updated signature to Guid)
        // ---------------------------------------------------------------------
        public async Task<EmailContextDto?> GetEmailContextAsync(Guid emailId)
        {
            // Invokes your business context builder using the strict Guid key tracking limits
            return await _builder.BuildAsync(emailId);
        }

        // ---------------------------------------------------------------------
        // 4. ARCHITECTURAL FORWARDERS
        // ---------------------------------------------------------------------
        public Task<List<OpenOrderDto>> GetLinkedOrdersAsync(Guid emailId)
        {
            throw new NotImplementedException(
                "Use GetEmailContextAsync instead (Email Intelligence Engine).");
        }

        public Task<List<TaskDto>> GetLinkedTasksAsync(Guid emailId)
        {
            throw new NotImplementedException(
                "Use GetEmailContextAsync instead (Email Intelligence Engine).");
        }
    }
}
