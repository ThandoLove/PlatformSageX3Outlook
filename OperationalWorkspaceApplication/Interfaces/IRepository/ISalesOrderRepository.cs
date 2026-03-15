using OperationalWorkspace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface ISalesOrderRepository
{
    System.Threading.Tasks.Task AddAsync(SalesOrder order, CancellationToken ct);
    Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<SalesOrder>> GetOpenOrdersAsync(string bpCode, CancellationToken ct);
}