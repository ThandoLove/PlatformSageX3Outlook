using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services;

public sealed class EmailContextBuilder
{
    private readonly IBusinessPartnerService _bpService;
    private readonly ISalesService _salesService;
    private readonly ITaskService _taskService;

    public EmailContextBuilder(
        IBusinessPartnerService bpService,
        ISalesService salesService,
        ITaskService taskService)
    {
        _bpService = bpService ?? throw new ArgumentNullException(nameof(bpService));
        _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    public async Task<EmailContextDto?> BuildAsync(Guid emailId)
    {
        if (emailId == Guid.Empty)
        {
            return null;
        }

        var ct = CancellationToken.None;

        var mockSenderEmail = "sarah.johnson@techinnovations.com";
        var partner = await _bpService.GetPartnerByEmailAsync(mockSenderEmail);

        if (partner == null)
        {
            var fallbackEmail = new { MessageId = emailId.ToString(), Subject = "Contextual Sync", From = mockSenderEmail, ReceivedAt = DateTime.Now, BodyPreview = "Requesting metrics..." };
            return BuildUnknown(fallbackEmail);
        }

        // 🚀 FIXED: All property calls (.Id and .AssignedToUserId) are now fully visible to the compiler!
        var openOrdersCount = await _salesService.CountOpenOrdersAsync(partner.AssignedToUserId);
        var assignedTasks = await _taskService.GetTasksAssignedToAsync(partner.AssignedToUserId);

        var linkedOrders = new List<LinkedOpenOrderDto>
        {
            new LinkedOpenOrderDto
            {
                OrderId = Guid.NewGuid().ToString(),
                OrderNumber = "SOH-100452",
                Status = "Open",
                CustomerName = partner.Company
            }
        };

        var linkedTasks = assignedTasks
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title
            })
            .ToList();

        return new EmailContextDto
        {
            Email = new EmailInsightDto
            {
                // 🚀 FIXED: Converted Guid safely to string to prevent implicit assignment compiler crashes
                MessageId = emailId.ToString(),
                Subject = "Urgent Request: Quote configuration updates",
                From = mockSenderEmail,
                ReceivedAt = DateTime.Now.AddMinutes(-10),
                Message = "Please verify the pricing thresholds inside our Sage X3 profile.",
                BusinessPartnerId = 0, // Maps to integer ID column safely
                BusinessPartnerName = partner.Company,
                BusinessPartnerCode = partner.BpCode,
                OpenOrderCount = openOrdersCount,
                OpenOrderValue = 12500m,
                OpenTaskCount = linkedTasks.Count,
                AssignedTaskCount = linkedTasks.Count,
                TotalOutstanding = partner.TotalOutstanding,
                OpenInvoiceCount = 0,
                HasOverdueInvoices = partner.OverdueInvoiceCount > 0,
                RiskLevel = "Normal",
                AccountStatus = "Active",
                IsOnCreditHold = partner.TotalOutstanding > partner.CreditLimit,
                HasBackOrders = false,
                HasLowStockImpact = false,
                SnapshotGeneratedAtUtc = DateTime.UtcNow,
                SenderEmail = mockSenderEmail,
                SenderName = partner.Company,
                To = string.Empty,
                AssignedUserId = partner.AssignedToUserId,
                ClientId = partner.Id // Successfully passes Guid to Guid match!
            },
            LinkedOrders = linkedOrders,
            LinkedTasks = linkedTasks
        };
    }

    private EmailContextDto BuildUnknown(dynamic email)
    {
        return new EmailContextDto
        {
            Email = new EmailInsightDto
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
            },
            LinkedOrders = new List<LinkedOpenOrderDto>(),
            LinkedTasks = new List<TaskDto>()
        };
    }
}
