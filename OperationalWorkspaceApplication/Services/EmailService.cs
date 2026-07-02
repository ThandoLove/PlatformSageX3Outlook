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
        // 2. GET BASIC EMAIL (Updated parameter to matching string contract format)
        // ---------------------------------------------------------------------
        public async Task<EmailInsightDto?> GetEmailByIdAsync(string outlookMessageId)
        {
            var email = await _repo.GetByMessageIdAsync(outlookMessageId);

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
        // 3. ⭐ REAL INTELLIGENCE ENTRY POINT (Updated signature to string)
        // ---------------------------------------------------------------------
        public async Task<EmailContextDto?> GetEmailContextAsync(string outlookMessageId)
        {
            if (string.IsNullOrWhiteSpace(outlookMessageId)) return null;

            // The builder expects a GUID for demo/mock lookups. If the stored email maps to a Guid client id
            // we attempt to resolve it. Otherwise we pass the special mock token for development flows.
            var stored = await _repo.GetByMessageIdAsync(outlookMessageId);

            if (stored == null)
            {
                return null;
            }

            // Try parse MessageId as Guid (if you store a GUID mapping); otherwise use a fixed demo Guid.
            if (Guid.TryParse(stored.MessageId, out Guid parsedGuid))
            {
                return await _builder.BuildAsync(parsedGuid);
            }

            // Fallback: builder uses a mock partner lookup via AssignedToUserId; we use stored.Id as client id
            return await _builder.BuildAsync(stored.Id);
        }

        // ---------------------------------------------------------------------
        // 4. ARCHITECTURAL FORWARDERS
        // ---------------------------------------------------------------------
        public Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string outlookMessageId)
        {
            throw new NotImplementedException(
                "Use GetEmailContextAsync instead (Email Intelligence Engine).");
        }

        public Task<List<TaskDto>> GetLinkedTasksAsync(string outlookMessageId)
        {
            throw new NotImplementedException(
                "Use GetEmailContextAsync instead (Email Intelligence Engine).");
        }
    }
}
