using OperationalWorkspace.Domain.Entities;
using Task = System.Threading.Tasks.Task;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct);
    Task<IReadOnlyList<InventoryItem>> GetByWarehouseAsync(string warehouseCode, CancellationToken ct);
    Task AddAsync(InventoryItem item, CancellationToken ct);
    Task UpdateAsync(InventoryItem item, CancellationToken ct);
    Task DeleteAsync(InventoryItem item, CancellationToken ct);

    // Only this stays here
    Task<int> GetStockAlertCountAsync(CancellationToken ct = default);
}
