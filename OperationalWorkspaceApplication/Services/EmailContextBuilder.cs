using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services
{
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
            _emailRepo = emailRepo ?? throw new ArgumentNullException(nameof(emailRepo));
            _salesOrderRepo = salesOrderRepo ?? throw new ArgumentNullException(nameof(salesOrderRepo));
            _taskRepo = taskRepo ?? throw new ArgumentNullException(nameof(taskRepo));
            _bpRepo = bpRepo ?? throw new ArgumentNullException(nameof(bpRepo));
        }

        public async Task<EmailContextDto?> BuildAsync(string emailId)
        {
            if (string.IsNullOrWhiteSpace(emailId))
            {
                return null;
            }

            var ct = CancellationToken.None;

            var email = await _emailRepo.GetByMessageIdAsync(emailId);

            if (email == null)
            {
                return null;
            }

            var bp = await _bpRepo.GetByEmailAsync(email.From, ct);

            if (bp == null)
            {
                return BuildUnknown(email);
            }

            var orders = (await _salesOrderRepo.GetOpenOrdersAsync(bp.BpCode, ct))
                ?.ToList()
                ?? new List<SalesOrder>();

            var tasks = (await _taskRepo.GetByUserAsync(bp.AssignedToUserId ?? string.Empty, ct))
                ?.ToList()
                ?? new List<TaskEntity>();

            var linkedOrders = orders
                .Select(o => new LinkedOpenOrderDto
                {
                    OrderId = o.Id.ToString(),
                    OrderNumber = o.OrderNumber.ToString(),
                    Status = o.Status.ToString(),
                    CustomerName = bp.Company
                })
                .ToList();

            var linkedTasks = tasks
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title
                })
                .ToList();

            var openOrderCount = orders.Count;
            var openOrderValue = orders.Sum(o => o.TotalAmount);

            return new EmailContextDto
            {
                Email = new EmailInsightDto
                {
                    MessageId = email.MessageId,
                    Subject = email.Subject,
                    From = email.From,
                    ReceivedAt = email.ReceivedAt,
                    Message = email.BodyPreview,
                    BusinessPartnerId = bp.Id,
                    BusinessPartnerName = bp.Company,
                    BusinessPartnerCode = bp.BpCode,
                    OpenOrderCount = openOrderCount,
                    OpenOrderValue = openOrderValue,
                    OpenTaskCount = tasks.Count,
                    AssignedTaskCount = tasks.Count,
                    TotalOutstanding = bp.OverdueInvoices,
                    OpenInvoiceCount = 0,
                    HasOverdueInvoices = bp.OverdueInvoices > 0,
                    RiskLevel = bp.IsActive ? "Normal" : "High",
                    AccountStatus = bp.IsActive ? "Active" : "Inactive",
                    IsOnCreditHold = bp.OverdueInvoices > bp.CreditLimit,
                    HasBackOrders = orders.Any(o => o.Status == SalesOrderStatus.Cancelled),
                    HasLowStockImpact = false,
                    SnapshotGeneratedAtUtc = DateTime.UtcNow,
                    SenderEmail = email.From,
                    SenderName = bp.Company,
                    To = string.Empty,
                    AssignedUserId = bp.AssignedToUserId ?? string.Empty,
                    ClientId = Guid.NewGuid()
                },
                LinkedOrders = linkedOrders,
                LinkedTasks = linkedTasks
            };
        }

        // =========================================================
        // UNKNOWN / UNMATCHED EMAILS
        // =========================================================
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
}
