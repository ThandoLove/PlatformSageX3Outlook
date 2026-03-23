using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using Task = System.Threading.Tasks.Task;


namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IInvoiceRepository
{
    Task<IReadOnlyList<Invoice>> GetOpenInvoicesByBpAsync(Guid businessPartnerId);
    Task<IEnumerable<object>> GetOpenInvoicesByBpAsync(object businessPartnerId);
    // Inside IInvoiceRepository.cs
    Task<Invoice> CreateInvoiceAsync(InvoiceDto dto);

    Task<Invoice?> GetByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetPagedAsync(int page, int pageSize);
    Task AddAsync(Invoice invoice);
    Task<Invoice> CreateFromOrderAsync(Guid orderId);
    Task<IReadOnlyList<Invoice>> GetByBusinessPartnerAsync(string bpCode, CancellationToken ct);
    Task<decimal> GetTotalOutstandingAmountAsync(string userId);
    Task<decimal> GetMonthlySalesAsync(string userId);
    Task<decimal> GetTotalOutstandingAmountAsync();
    Task<int> GetOverdueCountAsync();
    Task<int> GetGeneratedCountAsync();
    Task<int> GetDueSoonCountAsync(string userId);
    Task<int> GetHighRiskAccountsCountAsync();
    Task<decimal> GetTotalMonthlySalesAsync();



}



