using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using OperationalWorkspace.Domain.Entities;


namespace OperationalWorkspaceApplication.Services;

public class FinancialService : IFinancialService
{
    private readonly IInvoiceRepository _invoiceRepo;

    public FinancialService(IInvoiceRepository invoiceRepo)
    {
        _invoiceRepo = invoiceRepo;
    }

    public async Task<AgingReportResponse> GetAgingReportAsync(
        GetAgingReportRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Get Invoices (Note: converting string ID to Guid if necessary)
        IReadOnlyList<Invoice> invoices = (IReadOnlyList<Invoice>)await _invoiceRepo.GetOpenInvoicesByBpAsync(request.BusinessPartnerId);

        var today = DateTime.UtcNow;
        var buckets = new AgingSummaryDto();

        // 2. Logic (Bucket assignment)
        foreach (var invoice in invoices)
        {
            var daysOverdue = (today - invoice.DueDate).Days;
            var amount = invoice.OutstandingAmount;

            if (daysOverdue <= 0) buckets.Current += amount;
            else if (daysOverdue <= 30) buckets.Bucket30 += amount;
            else if (daysOverdue <= 60) buckets.Bucket60 += amount;
            else if (daysOverdue <= 90) buckets.Bucket90 += amount;
            else buckets.Bucket120Plus += amount;
        }

        buckets.Total = buckets.Current + buckets.Bucket30 + buckets.Bucket60 +
                        buckets.Bucket90 + buckets.Bucket120Plus;

        return new AgingReportResponse(buckets);
    }

    public async Task<RegisterPaymentResponse> RegisterPaymentAsync(
        RegisterPaymentRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement payment registration logic
        return new RegisterPaymentResponse(true, "Payment registered successfully.");
    }
}
