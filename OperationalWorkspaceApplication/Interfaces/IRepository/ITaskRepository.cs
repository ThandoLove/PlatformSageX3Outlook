
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks; // This is for the async 'Task' keyword
using OperationalWorkspace.Domain.Entities; // This is for your 'Task' entity

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface ITaskRepository
{
    // Use System.Threading.Tasks.Task for the return type
    // Use OperationalWorkspace.Domain.Entities.Task for the parameter
    System.Threading.Tasks.Task AddAsync(OperationalWorkspace.Domain.Entities.Task task, CancellationToken ct);

    System.Threading.Tasks.Task<OperationalWorkspace.Domain.Entities.Task?> GetByIdAsync(Guid id, CancellationToken ct);

    System.Threading.Tasks.Task UpdateAsync(OperationalWorkspace.Domain.Entities.Task task, CancellationToken ct);

    System.Threading.Tasks.Task<IReadOnlyList<OperationalWorkspace.Domain.Entities.Task>> GetByUserAsync(string userId, CancellationToken ct);
}
