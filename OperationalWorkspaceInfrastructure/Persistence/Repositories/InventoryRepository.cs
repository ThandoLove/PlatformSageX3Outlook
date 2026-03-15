using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly IntegrationDbContext _db;
    public InventoryRepository(IntegrationDbContext db) => _db = db;

    // ADD THIS: Missing GetByIdAsync
    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Inventories.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct)
        => await _db.Inventories.FirstOrDefaultAsync(i => i.ItemCode == sku, ct);

    // ADD THIS: Missing GetByWarehouseAsync
    public async Task<IReadOnlyList<InventoryItem>> GetByWarehouseAsync(string warehouseCode, CancellationToken ct)
        => await _db.Inventories
            .Where(i => i.WarehouseCode == warehouseCode)
            .ToListAsync(ct);

    // ADD THIS: Missing AddAsync
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
}
