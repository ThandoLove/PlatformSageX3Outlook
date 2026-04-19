using Microsoft.EntityFrameworkCore;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceInfrastructure.Persistence;
using Entities = OperationalWorkspace.Domain.Entities;
using Enums = OperationalWorkspace.Domain.Enums; // Added for Enum access
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IntegrationDbContext _db;
    public TaskRepository(IntegrationDbContext db) => _db = db;

    public async System.Threading.Tasks.Task AddAsync(Entities.TaskEntity task, CancellationToken ct)
    {
        await _db.Tasks.AddAsync(task, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async System.Threading.Tasks.Task<Entities.TaskEntity?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async System.Threading.Tasks.Task UpdateAsync(Entities.TaskEntity task, CancellationToken ct)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync(ct);
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<Entities.TaskEntity>> GetByUserAsync(string userId, CancellationToken ct)
    {
        // Convert string to Guid to match your Entity property type
        if (!Guid.TryParse(userId, out var userGuid)) return new List<Entities.TaskEntity>();

        return await _db.Tasks
            .Where(t => t.AssignedToUserId == userGuid)
            .ToListAsync(ct);
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<Entities.TaskEntity>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Tasks.ToListAsync(ct);
    }

    public async System.Threading.Tasks.Task<IReadOnlyList<Entities.TaskEntity>> GetPendingApprovalsAsync(string? userId, CancellationToken ct)
    {
        // FIX: Compare against the Enum value, not a string
        var query = _db.Tasks.Where(t => t.Status == Enums.TaskStatus.PendingApproval);

        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            query = query.Where(t => t.AssignedToUserId == userGuid);
        }

        return await query.ToListAsync(ct);
    }
}
