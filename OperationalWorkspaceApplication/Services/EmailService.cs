using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;

namespace OperationalWorkspaceApplication.Services;

public sealed class EmailService : IEmailService
{
    private readonly IEmailRepository _repo;
    private readonly EmailContextBuilder _builder;

    public EmailService(
        IEmailRepository repo,
        EmailContextBuilder builder)
    {
        _repo = repo;
        _builder = builder;
    }

    // ----------------------------
    // 1. STORE EMAIL
    // ----------------------------
    public async Task<bool> SyncEmailAsync(EmailInsightDto dto)
    {
        if (await _repo.ExistsAsync(dto.MessageId))
            return false;

        await _repo.AddAsync(new Email
        {
            MessageId = dto.MessageId,
            Subject = dto.Subject,
            From = dto.From,
            ReceivedAt = dto.ReceivedAt
        });

        return true;
    }

    // ----------------------------
    // 2. GET BASIC EMAIL
    // ----------------------------
    public async Task<EmailInsightDto?> GetEmailByIdAsync(string emailId)
    {
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

    // ----------------------------
    // 3. ⭐ REAL INTELLIGENCE ENTRY POINT
    // ----------------------------
    public async Task<EmailInsightDto?> GetEmailContextAsync(string emailId)
    {
        return await _builder.BuildAsync(emailId);
    }

    // ----------------------------
    // 4. REMOVED FAKE LOGIC (IMPORTANT)
    // ----------------------------

    public Task<List<OpenOrderDto>> GetLinkedOrdersAsync(string emailId)
    {
        throw new NotImplementedException(
            "Use GetEmailContextAsync instead (Email Intelligence Engine).");
    }

    public Task<List<TaskDto>> GetLinkedTasksAsync(string emailId)
    {
        throw new NotImplementedException(
            "Use GetEmailContextAsync instead (Email Intelligence Engine).");
    }
}