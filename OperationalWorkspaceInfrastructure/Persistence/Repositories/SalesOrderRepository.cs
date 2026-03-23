using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspace.Domain.Enums; // Required for SalesOrderStatus
using OperationalWorkspaceApplication.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly IntegrationDbContext _db;
        public SalesOrderRepository(IntegrationDbContext db) => _db = db;

        // Use System.Threading.Tasks.Task to satisfy the interface void-task
        public async System.Threading.Tasks.Task AddAsync(SalesOrder order, CancellationToken ct)
        {
            await _db.SalesOrders.AddAsync(order, ct);
            await _db.SaveChangesAsync(ct);
        }

        // Explicitly use Task<T> for methods with return values
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
            // Parse string to Enum to fix the '==' error from the previous step
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
}
