using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
// Explicitly alias Task to the System version to avoid conflict with your Domain.Task entity
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly IntegrationDbContext _db;
    public InvoiceRepository(IntegrationDbContext db) => _db = db;

    // Use System.Threading.Tasks.Task<T> to be safe
    public async System.Threading.Tasks.Task<Invoice?> GetByIdAsync(Guid id)
        => await _db.Invoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async System.Threading.Tasks.Task<IEnumerable<Invoice>> GetPagedAsync(int page, int pageSize)
        => await _db.Invoices.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    // FIX: Using the alias 'Task' (which is System.Threading.Tasks.Task)
    // This matches the Interface signature perfectly
    public async Task AddAsync(Invoice invoice)
    {
        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<Invoice>> GetByBusinessPartnerAsync(string bpCode, CancellationToken ct)
        => await _db.Invoices.AsNoTracking()
            .Where(i => i.BpCode == bpCode)
            .ToListAsync(ct);

    public async System.Threading.Tasks.Task<IReadOnlyList<Invoice>> GetOpenInvoicesByBpAsync(Guid businessPartnerId)
        => await _db.Invoices.AsNoTracking()
            .Where(i => i.BusinessPartnerId == businessPartnerId && i.Status != InvoiceStatus.Paid)
            .ToListAsync();

    public async System.Threading.Tasks.Task<IEnumerable<object>> GetOpenInvoicesByBpAsync(object businessPartnerId)
    {
        if (businessPartnerId is Guid guidId)
            return await GetOpenInvoicesByBpAsync(guidId);

        return Enumerable.Empty<object>();
    }

    // --- Dashboard & Analytics ---

    public async System.Threading.Tasks.Task<decimal> GetTotalOutstandingAmountAsync(string userId)
        => await _db.Invoices.Where(i => i.UserId == userId && i.Status != InvoiceStatus.Paid).SumAsync(i => i.Amount);

    public async System.Threading.Tasks.Task<decimal> GetTotalOutstandingAmountAsync()
        => await _db.Invoices.Where(i => i.Status != InvoiceStatus.Paid).SumAsync(i => i.Amount);

    public async System.Threading.Tasks.Task<decimal> GetMonthlySalesAsync(string userId)
    {
        var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        return await _db.Invoices.Where(i => i.UserId == userId && i.CreatedAt >= start).SumAsync(i => i.Amount);
    }

    public async System.Threading.Tasks.Task<decimal> GetTotalMonthlySalesAsync()
    {
        var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        return await _db.Invoices.Where(i => i.CreatedAt >= start).SumAsync(i => i.Amount);
    }

    public async System.Threading.Tasks.Task<int> GetOverdueCountAsync()
        => await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Overdue);

    public async System.Threading.Tasks.Task<int> GetGeneratedCountAsync()
        => await _db.Invoices.CountAsync();

    public async System.Threading.Tasks.Task<int> GetDueSoonCountAsync(string userId)
        => await _db.Invoices.CountAsync(i => i.UserId == userId && i.Status != InvoiceStatus.Paid);

    public async System.Threading.Tasks.Task<int> GetHighRiskAccountsCountAsync()
        => await _db.BusinessPartners.CountAsync(bp => bp.OverdueInvoices > bp.CreditLimit);

    // --- CRUD ---

    public async System.Threading.Tasks.Task<Invoice> CreateInvoiceAsync(InvoiceDto dto)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            Status = InvoiceStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UserId = "System" // Or pass current user ID
        };
        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public async System.Threading.Tasks.Task<Invoice> CreateFromOrderAsync(Guid orderId)
    {
        var order = await _db.SalesOrders.FindAsync(orderId);
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Amount = order?.TotalAmount ?? 0,
            Status = InvoiceStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            BusinessPartnerId = order?.BusinessPartnerId ?? Guid.Empty,
            BpCode = order?.BpCode ?? string.Empty,
            UserId = "System" // Assign the appropriate user ID
        };
        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }
}
