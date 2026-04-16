using OperationalWorkspaceApplication.IServices;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OperationalWorkspace.Application.Services;

public sealed class BusinessPartnerService : IBusinessPartnerService
{
    private readonly IBusinessPartnerRepository _partnerRepo;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ISalesOrderRepository _salesRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public BusinessPartnerService(
        IBusinessPartnerRepository partnerRepo,
        IInvoiceRepository invoiceRepo,
        ISalesOrderRepository salesRepo,
        IUnitOfWork uow,
        IClock clock,
        HttpClient httpClient,
        IConfiguration config)
    {
        _partnerRepo = partnerRepo;
        _invoiceRepo = invoiceRepo;
        _salesRepo = salesRepo;
        _uow = uow;
        _clock = clock;
        _httpClient = httpClient;
        _config = config;
    }

    // ==========================================
    // 🚀 NEW: CREATE CONTACT IN SAGE X3
    // ==========================================
    public async Task<bool> CreateContactAsync(ContactCreateDto contact)
    {
        try
        {
            var baseUrl = _config["SageX3:BaseUrl"];
            var endpoint = _config["SageX3:Endpoint"];
            var url = $"{baseUrl}/api1/x3/erp/{endpoint}/CONTACT?representation=CONTACT.$create";

            // Map the expanded DTO to Sage X3 fields
            var payload = new
            {
                CNPFNA = contact.FullName, // First Name
                CNPLNA = contact.FullName, // Last Name (X3 often needs both)
                WEB = contact.Email,
                TEL = contact.Phone,
                MOB = contact.Mobile,
                BPRNUM = contact.Company,   // Linked BP Code
                BPCAT = contact.Category,   // Category

                // Address Mapping (If your representation includes BPAADD)
                ADDRESS1 = contact.Street,
                CITY = contact.City,
                ZIP = contact.ZipCode,
                CRY = contact.Country
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }


    // ==========================================
    // 📧 CREATE CLIENT FROM EMAIL
    // ==========================================
    public async Task<CreateClientFromEmailResponse> CreateFromEmailAsync(
        CreateClientFromEmailRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");

        var existing = await _partnerRepo.GetByEmailAsync(request.Email, ct);

        if (existing != null)
        {
            return new CreateClientFromEmailResponse
            {
                Id = Guid.NewGuid(),
                Code = existing.BpCode
            };
        }

        return new CreateClientFromEmailResponse
        {
            Id = Guid.NewGuid(),
            Code = "TEMP-" + Guid.NewGuid().ToString().Substring(0, 6)
        };
    }

    // ==========================================
    // 🔍 SNAPSHOTS & DATA RETRIEVAL
    // ==========================================
    public async Task<BusinessPartnerSnapshotDto?> GetPartnerByEmailAsync(string? email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        var partner = await _partnerRepo.GetByEmailAsync(email!, ct);
        if (partner is null) return null;

        var invoices = await _invoiceRepo.GetByBusinessPartnerAsync(partner.BpCode, ct);
        var orders = await _salesRepo.GetOpenOrdersAsync(partner.BpCode, ct);

        var now = _clock.UtcNow;
        var overdue = invoices.Where(i => i.DueDate < now && i.OutstandingAmount > 0).ToList();

        return new BusinessPartnerSnapshotDto(
            partner.BpCode,
            partner.Company,
            partner.CreditLimit,
            invoices.Sum(i => i.OutstandingAmount),
            orders.Count,
            overdue.Count,
            overdue.Sum(i => i.OutstandingAmount),
            partner.LastContactDate)
        {
            FullName = partner.ContactName,
            IsLinkedToSage = true,
            Location = $"{partner.City}, {partner.State}",
            AssignedRep = partner.SalesRepName,
            Timeline = await GetRecentActivityTimeline(partner.BpCode, ct)
        };
    }

    private async Task<List<ActivityDto>> GetRecentActivityTimeline(string bpCode, CancellationToken ct)
    {
        return new List<ActivityDto>
        {
            new ActivityDto { Title = "Email Received", Action = "Quote Request", Timestamp = _clock.UtcNow },
            new ActivityDto { Title = "Meeting", Action = "Follow-up Scheduled", Timestamp = _clock.UtcNow.AddDays(-5) },
            new ActivityDto { Title = "Invoice", Action = "INV-2024-001 Generated", Timestamp = _clock.UtcNow.AddDays(-10) }
        };
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

    // ==========================================
    // 📊 DASHBOARD KPI METHODS
    // ==========================================
    public async Task<int> CountActiveCustomersAsync() => await _partnerRepo.GetActiveCountAsync();
    public async Task<int> CountNewLeadsTodayAsync() => await _partnerRepo.GetLeadsCreatedAfterAsync(_clock.UtcNow.Date);
    public async Task<int> CountOpenOpportunitiesAsync(string userId) => await _partnerRepo.GetOpenOpportunitiesCountAsync(userId);
    public async Task<int> CountOpenOpportunitiesAsync() => await _partnerRepo.GetOpenOpportunitiesCountAsync();

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

    public async Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(UpdateCreditLimitRequest request, CancellationToken ct)
    {
        var partner = await _partnerRepo.GetByCodeAsync(request.BpCode, ct);
        if (partner is null) return new UpdateCreditLimitResponse(false);

        partner.UpdateCreditLimit(request.NewLimit);
        await _partnerRepo.UpdateAsync(partner, ct);
        await _uow.SaveChangesAsync(ct);

        return new UpdateCreditLimitResponse(true);
    }
}
