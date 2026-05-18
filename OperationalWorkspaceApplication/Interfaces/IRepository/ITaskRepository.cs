using OperationalWorkspace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface ITaskRepository
{
    Task AddAsync(TaskEntity task, CancellationToken ct);

    Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken ct);

    Task UpdateAsync(TaskEntity task, CancellationToken ct);

    Task<IReadOnlyList<TaskEntity>> GetByUserAsync(string userId, CancellationToken ct);

    Task<IReadOnlyList<TaskEntity>> GetAllAsync(CancellationToken ct);

    Task<IReadOnlyList<TaskEntity>> GetPendingApprovalsAsync(string? userId, CancellationToken ct);
}