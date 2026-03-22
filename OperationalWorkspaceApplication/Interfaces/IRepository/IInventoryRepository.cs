using OperationalWorkspace.Domain.Entities;
using Task = System.Threading.Tasks.Task; // Force Task to mean the async keyword

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IInventoryRepository
{
    // Basic Persistence
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct);
    Task<IReadOnlyList<InventoryItem>> GetByWarehouseAsync(string warehouseCode, CancellationToken ct);

    // Command Operations
    Task AddAsync(InventoryItem item, CancellationToken ct);
    Task UpdateAsync(InventoryItem item, CancellationToken ct);
    Task DeleteAsync(InventoryItem item, CancellationToken ct);
    Task<int> GetStockAlertCountAsync(CancellationToken ct = default);
    Task<decimal> GetTotalOutstandingAmountAsync(string? userId = null);
    Task<decimal> GetMonthlySalesAsync(string userId);
    Task<int> GetOverdueCountAsync();
    Task<int> GetGeneratedCountAsync();
    Task<int> GetDueSoonCountAsync(string userId);
    Task<int> GetHighRiskAccountsCountAsync();

}
