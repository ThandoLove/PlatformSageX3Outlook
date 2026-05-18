using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services;

public sealed class EmailContextBuilder
{
    private readonly IEmailRepository _emailRepo;
    private readonly ISalesOrderRepository _salesOrderRepo;
    private readonly ITaskRepository _taskRepo;
    private readonly IBusinessPartnerRepository _bpRepo;

    public EmailContextBuilder(
        IEmailRepository emailRepo,
        ISalesOrderRepository salesOrderRepo,
        ITaskRepository taskRepo,
        IBusinessPartnerRepository bpRepo)
    {
        _emailRepo = emailRepo;
        _salesOrderRepo = salesOrderRepo;
        _taskRepo = taskRepo;
        _bpRepo = bpRepo;
    }

    public async Task<EmailInsightDto?> BuildAsync(string emailId)
    {
        var ct = CancellationToken.None;

        // ======================
        // 1. EMAIL
        // ======================
        var email = await _emailRepo.GetByMessageIdAsync(emailId);
        if (email == null)
            return null;

        // ======================
        // 2. BUSINESS PARTNER
        // ======================
        var bp = await _bpRepo.GetByEmailAsync(email.From, ct);

        if (bp == null)
        {
            return new EmailInsightDto
            {
                MessageId = email.MessageId,
                Subject = email.Subject,
                From = email.From,
                ReceivedAt = email.ReceivedAt,
                Message = email.BodyPreview,

                BusinessPartnerId = 0,
                BusinessPartnerName = "Unknown",
                BusinessPartnerCode = string.Empty,

                OpenOrderCount = 0,
                OpenOrderValue = 0,

                OpenTaskCount = 0,
                AssignedTaskCount = 0,

                TotalOutstanding = 0,
                OpenInvoiceCount = 0,
                HasOverdueInvoices = false,

                RiskLevel = "Unknown",
                AccountStatus = "Unknown",
                IsOnCreditHold = false,

                HasBackOrders = false,
                HasLowStockImpact = false,

                SnapshotGeneratedAtUtc = DateTime.UtcNow,

                SenderEmail = email.From,
                SenderName = email.From,

                To = string.Empty,
                AssignedUserId = string.Empty,
                ClientId = Guid.Empty
            };
        }

        // ======================
        // 3. ORDERS
        // ======================
        var orders = await _salesOrderRepo.GetOpenOrdersAsync(bp.BpCode, ct);

        // ======================
        // 4. TASKS
        // ======================
        var tasks = await _taskRepo.GetByUserAsync(bp.AssignedToUserId ?? string.Empty, ct);

        // ======================
        // 5. BUILD DTO
        // ======================
        return new EmailInsightDto
        {
            MessageId = email.MessageId,
            Subject = email.Subject,
            From = email.From,
            ReceivedAt = email.ReceivedAt,
            Message = email.BodyPreview,

            BusinessPartnerId = bp.Id,
            BusinessPartnerName = bp.Company,
            BusinessPartnerCode = bp.BpCode,

            OpenOrderCount = orders?.Count ?? 0,
            OpenOrderValue = orders?.Sum(o => o.TotalAmount) ?? 0,

            OpenTaskCount = tasks?.Count ?? 0,
            AssignedTaskCount = tasks?.Count ?? 0,

            TotalOutstanding = bp.OverdueInvoices,
            OpenInvoiceCount = 0,
            HasOverdueInvoices = bp.OverdueInvoices > 0,

            RiskLevel = bp.IsActive ? "Normal" : "High",
            AccountStatus = bp.IsActive ? "Active" : "Inactive",
            IsOnCreditHold = bp.OverdueInvoices > bp.CreditLimit,

            HasBackOrders = false,
            HasLowStockImpact = false,

            SnapshotGeneratedAtUtc = DateTime.UtcNow,

            SenderEmail = email.From,
            SenderName = bp.Company,

            To = string.Empty,
            AssignedUserId = bp.AssignedToUserId ?? string.Empty,
            // ClientId remains a Guid for client correlation; producing a new Guid
            // if this business partner exists.
            ClientId = bp.Id == 0 ? Guid.Empty : Guid.NewGuid()
        };
    }
}