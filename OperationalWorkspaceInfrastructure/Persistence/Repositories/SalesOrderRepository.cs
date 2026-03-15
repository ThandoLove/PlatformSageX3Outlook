using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
// This alias is critical to resolve the conflict with your Domain.Entities.Task
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly IntegrationDbContext _db;
        public SalesOrderRepository(IntegrationDbContext db) => _db = db;

        // Matches: Task AddAsync(SalesOrder order, CancellationToken ct)
        public async Task AddAsync(SalesOrder order, CancellationToken ct)
        {
            await _db.SalesOrders.AddAsync(order, ct);
            await _db.SaveChangesAsync(ct);
        }

        // Matches: Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken ct)
        public async Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _db.SalesOrders.FirstOrDefaultAsync(o => o.Id == id, ct);

        // Matches: Task<IReadOnlyList<SalesOrder>> GetOpenOrdersAsync
        public async Task<IReadOnlyList<SalesOrder>> GetOpenOrdersAsync(string bpCode, CancellationToken ct)
        {
            return await _db.SalesOrders
                .Where(o => o.BpCode == bpCode && !o.IsClosed)
                .ToListAsync(ct);
        }
    }
}
