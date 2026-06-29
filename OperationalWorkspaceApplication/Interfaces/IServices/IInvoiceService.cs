using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize);

    Task<InvoiceDto?> GetByIdAsync(Guid id);

    Task<int> CountOverdueInvoicesAsync();

    Task<int> CountInvoicesGeneratedAsync();

    Task<int> CountInvoicesDueAsync(string userId);

    Task<decimal> GetOutstandingInvoiceValueAsync();

    Task<decimal> GetUserOutstandingInvoiceValueAsync(string userId);

    Task<decimal> GetCurrentMonthInvoiceValueAsync();

    Task<decimal> GetUserCurrentMonthInvoiceValueAsync(string userId);
}