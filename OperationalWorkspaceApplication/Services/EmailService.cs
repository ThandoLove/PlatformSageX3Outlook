using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly IEmailRepository _repo;
        public EmailService(IEmailRepository repo) => _repo = repo;

        public async Task<bool> SyncEmailAsync(EmailInsightDto dto)
        {
            if (await _repo.ExistsAsync(dto.MessageId)) return false; // Idempotency Shield
            await _repo.AddAsync(new Email { MessageId = dto.MessageId, Subject = dto.Subject, From = dto.From, ReceivedAt = dto.ReceivedAt });
            return true;
        }
    }
}
