using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceInfrastructure.Persistence;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly IntegrationDbContext _db;

    public InvoiceRepository(IntegrationDbContext db) => _db = db;

    public async System.Threading.Tasks.Task<Invoice?> GetByIdAsync(Guid id)
        => await _db.Invoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async System.Threading.Tasks.Task<IEnumerable<Invoice>> GetPagedAsync(int page, int pageSize)
        => await _db.Invoices.AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

    public async System.Threading.Tasks.Task AddAsync(Invoice invoice)
    {
        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();
    }

    // FIX: Added to satisfy IInvoiceRepository
    public async System.Threading.Tasks.Task<Invoice> CreateInvoiceAsync(InvoiceDto dto)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BusinessPartnerId = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            InvoiceNumber = dto.InvoiceNumber,
            Amount = dto.Amount,
            Status = InvoiceStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public async System.Threading.Tasks.Task<Invoice> CreateFromOrderAsync(Guid orderId)
    {
        var order = await _db.SalesOrders.AsNoTracking()
                             .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new KeyNotFoundException($"Order {orderId} not found.");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BusinessPartnerId = order.BusinessPartnerId,
            BpCode = order.BpCode ?? string.Empty,
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{orderId.ToString()[..4].ToUpper()}",
            Amount = order.TotalAmount,
            Status = InvoiceStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();

        return invoice;
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<Invoice>> GetByBusinessPartnerAsync(string bpCode, CancellationToken ct)
        => await _db.Invoices.AsNoTracking().Where(i => i.BpCode == bpCode).ToListAsync(ct);

    public async System.Threading.Tasks.Task<IReadOnlyList<Invoice>> GetOpenInvoicesByBpAsync(Guid bpId)
        => await _db.Invoices.AsNoTracking()
                    .Where(i => i.BusinessPartnerId == bpId && i.Status != InvoiceStatus.Paid)
                    .ToListAsync();

    public async System.Threading.Tasks.Task<IEnumerable<object>> GetOpenInvoicesByBpAsync(object bpIdentifier)
    {
        if (bpIdentifier is Guid guidId) return (IEnumerable<object>)await GetOpenInvoicesByBpAsync(guidId);
        if (bpIdentifier is string code) return (IEnumerable<object>)await GetByBusinessPartnerAsync(code, default);
        return Enumerable.Empty<object>();
    }
}
