

using OperationalWorkspace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface ISalesOrderRepository
{
    Task AddAsync(SalesOrder order, CancellationToken ct);

    Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<SalesOrder>> GetOpenOrdersAsync(string bpCode, CancellationToken ct);

    Task<int> CountByStatusAsync(string bpCode, string status, CancellationToken ct);

    Task<int> CountTotalAsync(CancellationToken ct);
}