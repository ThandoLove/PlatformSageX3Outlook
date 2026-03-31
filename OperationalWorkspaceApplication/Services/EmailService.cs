using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Linq; // Required for .Select()
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly IEmailRepository _repo;

        public EmailService(IEmailRepository repo) => _repo = repo;

        public async System.Threading.Tasks.Task<bool> SyncEmailAsync(EmailInsightDto dto)
        {
            if (await _repo.ExistsAsync(dto.MessageId)) return false;

            await _repo.AddAsync(new Email
            {
                MessageId = dto.MessageId,
                Subject = dto.Subject,
                From = dto.From,
                ReceivedAt = dto.ReceivedAt
            });

            return true;
        }

        public async System.Threading.Tasks.Task<EmailInsightDto?> GetEmailByIdAsync(string emailId)
        {
            var email = await _repo.GetByMessageIdAsync(emailId);
            if (email == null) return null;

            return new EmailInsightDto
            {
                MessageId = email.MessageId,
                Subject = email.Subject,
                From = email.From,
                ReceivedAt = email.ReceivedAt
            };
        }

        // FIX: This method now explicitly handles the conversion to List<OpenOrderDto>
        public async System.Threading.Tasks.Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId)
        {
            // When you actually implement this, you'll fetch OrderDto and map it here
            return await System.Threading.Tasks.Task.FromResult(new List<OpenOrderDto>());
        }

        public async System.Threading.Tasks.Task<List<TaskDto>> GetLinkedTasksAsync(string emailId)
        {
            return await System.Threading.Tasks.Task.FromResult(new List<TaskDto>());
        }
    }
}
