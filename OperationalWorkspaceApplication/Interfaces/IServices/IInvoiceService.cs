using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IInvoiceService
    {
        // Shield: Use concrete DTOs so the Controller can access 'Id' and other properties
        Task<InvoiceDto> CreateFromOrderAsync(Guid orderId);

        // FIX: Changed from Task<object> to Task<InvoiceDto>
        Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto);

        // FIX: Changed from Task<object> to Task<IEnumerable<InvoiceDto>>
        // This allows the API to return a proper JSON array of invoices
        Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize);

        Task<InvoiceDto?> GetByIdAsync(Guid id);
    }
}
