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

    public async Task<int> CountActiveCustomersAsync()
        => await _partnerRepo.GetActiveCountAsync();

    public async Task<int> CountNewLeadsTodayAsync()
        => await _partnerRepo.GetLeadsCreatedAfterAsync(_clock.UtcNow.Date);

    public async Task<int> CountOpenOpportunitiesAsync(string userId)
        => await _partnerRepo.GetOpenOpportunitiesCountAsync(userId);

    public async Task<int> CountOpenOpportunitiesAsync()
        => await _partnerRepo.GetOpenOpportunitiesCountAsync();

    public async Task<string> GetRecentInteractionAsync(string userId)
    {
        var interaction = await _partnerRepo.GetLatestInteractionNoteAsync(userId);
        return interaction ?? "No recent interactions";
    }

    public async Task<string?> GetTopCustomerAsync(string userId)
    {
        var partner = await _partnerRepo.GetTopCustomerBySalesAsync(userId);
        return partner?.Company;
    }

    public async Task<BusinessPartnersResponse?> GetSnapshotAsync(
        GetBusinessPartnerSnapshotRequest request,
        CancellationToken ct)
    {
        var partner = await _partnerRepo.GetByCodeAsync(request.BpCode, ct);
        if (partner is null) return null;

        var invoices = await _invoiceRepo.GetByBusinessPartnerAsync(request.BpCode, ct);
        var orders = await _salesRepo.GetOpenOrdersAsync(request.BpCode, ct);

        var now = _clock.UtcNow;
        var overdue = invoices.Where(i => i.DueDate < now && i.OutstandingAmount > 0).ToList();

        var dto = new BusinessPartnerSnapshotDto(
            partner.BpCode,
            partner.Company,
            partner.CreditLimit,
            invoices.Sum(i => i.OutstandingAmount),
            orders.Count,
            overdue.Count,
            overdue.Sum(i => i.OutstandingAmount),
            partner.LastContactDate);

        return new BusinessPartnersResponse(dto);
    }

    public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(
        UpdateCreditLimitRequest request,
        CancellationToken ct)
    {
        var partner = await _partnerRepo.GetByCodeAsync(request.BpCode, ct);
        if (partner is null) return new UpdateCreditLimitResponse(false);

        partner.UpdateCreditLimit(request.NewLimit);
        await _partnerRepo.UpdateAsync(partner, ct);
        await _uow.SaveChangesAsync(ct);

        return new UpdateCreditLimitResponse(true);
    }
}
