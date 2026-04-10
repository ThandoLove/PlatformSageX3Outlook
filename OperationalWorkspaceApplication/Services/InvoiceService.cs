using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.IServices;

namespace OperationalWorkspaceApplication.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly ILogger<InvoiceService> _logger;
    private readonly IClock _clock;

    public InvoiceService(
        IInvoiceRepository repository,
        ILogger<InvoiceService> logger,
        IClock clock)
    {
        _repository = repository;
        _logger = logger;
        _clock = clock;
    }

    // --- existing methods (GetById, GetAll, Create, etc.) ---

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice == null) return null;

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber.ToString(),
            Amount = invoice.Amount,
            Status = invoice.Status.ToString()
        };
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page, int pageSize)
    {
        var invoices = await _repository.GetPagedAsync(page, pageSize);
        return invoices.Select(i => new InvoiceDto
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber.ToString(),
            Amount = i.Amount,
            Status = i.Status.ToString()
        });
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto)
    {
        var entity = new Invoice
        {
            Amount = dto.Amount,
            Status = InvoiceStatus.Draft
        };

        await _repository.AddAsync(entity);
        _logger.LogInformation("Invoice {Id} created successfully.", entity.Id);

        dto.Id = entity.Id;
        dto.Status = entity.Status.ToString();
        return dto;
    }

    public async Task<InvoiceDto> CreateFromOrderAsync(Guid orderId)
    {
        var invoice = await _repository.CreateFromOrderAsync(orderId);
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber.ToString(),
            Amount = invoice.Amount,
            Status = invoice.Status.ToString()
        };
    }

    // --- Dashboard & Reporting Logic ---

    public async Task<decimal> GetOutstandingReceivablesAsync()
        => await _repository.GetTotalOutstandingAmountAsync();

    public async Task<decimal> GetOutstandingReceivablesAsync(string userId)
        => await _repository.GetTotalOutstandingAmountAsync(userId);

    public async Task<decimal> GetMonthlySalesAsync(string userId)
        => await _repository.GetMonthlySalesAsync(userId);

    public async Task<int> CountOverdueInvoicesAsync()
        => await _repository.GetOverdueCountAsync();

    public async Task<int> CountInvoicesGeneratedAsync()
        => await _repository.GetGeneratedCountAsync();

    public async Task<int> CountInvoicesDueAsync(string userId)
        => await _repository.GetDueSoonCountAsync(userId);

    public async Task<int> CountHighRiskAccountsAsync()
        => await _repository.GetHighRiskAccountsCountAsync();

    // ============================================================
    // FIX: Missing Interface Implementations for Admin Dashboard
    // ============================================================

    public async Task<decimal> GetTotalOutstandingReceivablesAsync()
    {
        // Calls the global repository method (no userId filter)
        return await _repository.GetTotalOutstandingAmountAsync();
    }

    public async Task<decimal> GetTotalMonthlySalesAsync()
    {
        // Assuming your repository has a global version of this method
        // If not, you may need to add it to IInvoiceRepository as well
        return await _repository.GetTotalMonthlySalesAsync();
    }
}
