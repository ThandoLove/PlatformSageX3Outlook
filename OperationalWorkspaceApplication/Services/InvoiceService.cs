using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums; // Fix: Ensure this namespace is imported

namespace OperationalWorkspaceApplication.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(IInvoiceRepository repository, ILogger<InvoiceService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice == null) return null;

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber.ToString(), // Fix: Ensure string type
            Amount = invoice.Amount,
            Status = invoice.Status.ToString() // Fix: Convert Enum to String
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
            Status = i.Status.ToString() // Fix: Convert Enum to String
        });
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceDto dto)
    {
        var entity = new Invoice
        {
            // Fix: Map from DTO and set proper Enum status
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
            Status = invoice.Status.ToString() // Fix: Convert Enum to String
        };
    }
}
