using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums; // Required for SalesOrderStatus
using OperationalWorkspaceApplication.Interfaces.IRepository;


namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly IntegrationDbContext _db;
    public SalesOrderRepository(IntegrationDbContext db) => _db = db;

    // REMOVED: AddAsync implementation entirely

    public async Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.SalesOrders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<SalesOrder>> GetOpenOrdersAsync(string bpCode, CancellationToken ct)
    {
        return await _db.SalesOrders
            .Where(o => o.BpCode == bpCode && !o.IsClosed)
            .ToListAsync(ct);
    }

    public async Task<int> CountByStatusAsync(string bpCode, string status, CancellationToken ct)
    {
        if (!Enum.TryParse<SalesOrderStatus>(status, true, out var statusEnum))
        {
            return 0;
        }

        return await _db.SalesOrders
            .CountAsync(o => o.BpCode == bpCode && o.Status == statusEnum, ct);
    }

    public async Task<int> CountTotalAsync(CancellationToken ct)
    {
        return await _db.SalesOrders.CountAsync(ct);
    }
}
