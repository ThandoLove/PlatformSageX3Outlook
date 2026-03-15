using OperationalWorkspaceApplication.Abstractions;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;

namespace OperationalWorkspace.Application.Services;

public sealed class BusinessPartnerService : IBusinessPartnerService
{
    private readonly IBusinessPartnerRepository _partnerRepo;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ISalesOrderRepository _salesRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public BusinessPartnerService(
        IBusinessPartnerRepository partnerRepo,
        IInvoiceRepository invoiceRepo,
        ISalesOrderRepository salesRepo,
        IUnitOfWork uow,
        IClock clock)
    {
        _partnerRepo = partnerRepo;
        _invoiceRepo = invoiceRepo;
        _salesRepo = salesRepo;
        _uow = uow;
        _clock = clock;
    }

    // FIX: Return Task<BusinessPartnersResponse?> to match Interface
    public async Task<BusinessPartnersResponse?> GetSnapshotAsync(
        GetBusinessPartnerSnapshotRequest request,
        CancellationToken ct)
    {
        var partner = await _partnerRepo.GetByCodeAsync(request.BpCode, ct);
        if (partner is null) return null;

        var invoices = await _invoiceRepo.GetByBusinessPartnerAsync(request.BpCode, ct);
        var orders = await _salesRepo.GetOpenOrdersAsync(request.BpCode, ct);

        var now = _clock.UtcNow;

        var overdue = invoices
            .Where(i => i.DueDate < now && i.OutstandingAmount > 0)
            .ToList();

        var dto = new BusinessPartnerSnapshotDto(
            partner.BpCode,
            partner.Company,
            partner.CreditLimit,
            invoices.Sum(i => i.OutstandingAmount),
            orders.Count,
            overdue.Count,
            overdue.Sum(i => i.OutstandingAmount),
            partner.LastContactDate);

        // FIX: Return the specific Response record
        return new BusinessPartnersResponse(dto);
    }

    public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(
        UpdateCreditLimitRequest request,
        CancellationToken ct)
    {
        var partner = await _partnerRepo.GetByCodeAsync(request.BpCode, ct);
        if (partner is null) return new UpdateCreditLimitResponse(false);

        // FIX: This now exists in the Domain Entity
        partner.UpdateCreditLimit(request.NewLimit);

        await _partnerRepo.UpdateAsync(partner, ct);
        await _uow.SaveChangesAsync(ct);

        return new UpdateCreditLimitResponse(true);
    }
}
