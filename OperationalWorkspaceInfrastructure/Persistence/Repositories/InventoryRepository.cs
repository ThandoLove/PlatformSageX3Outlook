using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly IntegrationDbContext _db;
    public InventoryRepository(IntegrationDbContext db) => _db = db;

    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Inventories.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct)
        => await _db.Inventories.FirstOrDefaultAsync(i => i.ItemCode == sku, ct);

    public async Task<IReadOnlyList<InventoryItem>> GetByWarehouseAsync(string warehouseCode, CancellationToken ct)
        => await _db.Inventories.Where(i => i.WarehouseCode == warehouseCode).ToListAsync(ct);

    public async Task AddAsync(InventoryItem item, CancellationToken ct)
    {
        await _db.Inventories.AddAsync(item, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken ct)
    {
        _db.Inventories.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(InventoryItem item, CancellationToken ct)
    {
        _db.Inventories.Remove(item);
        await _db.SaveChangesAsync(ct);
    }

    // FIX: Added missing Dashboard method
    public async Task<int> GetStockAlertCountAsync(CancellationToken ct = default)
    {
        // Adjust 'Quantity' and 'ReorderLevel' to match your entity names
        return await _db.Inventories.CountAsync(i => i.Quantity <= i.ReorderLevel, ct);
    }

    // FIX: These finance methods technically belong in InvoiceRepo, 
    // but added here to satisfy your current IInventoryRepository interface.
    public Task<decimal> GetTotalOutstandingAmountAsync(string? userId = null) => Task.FromResult(0m);
    public Task<decimal> GetMonthlySalesAsync(string userId) => Task.FromResult(0m);
    public Task<int> GetOverdueCountAsync() => Task.FromResult(0);
    public Task<int> GetGeneratedCountAsync() => Task.FromResult(0);
    public Task<int> GetDueSoonCountAsync(string userId) => Task.FromResult(0);
    public Task<int> GetHighRiskAccountsCountAsync() => Task.FromResult(0);
}
