using System.Linq;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
// Changed: Use a cleaner alias
using DomainTask = OperationalWorkspace.Domain.Entities.Task;
using DomainStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceApplication.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IAuditLogRepository _audit;

    public TaskService(ITaskRepository repo, IAuditLogRepository audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async System.Threading.Tasks.Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var task = new DomainTask // Use the alias
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedBy = request.CreatedBy,
            Status = DomainStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(task, ct);
        return TaskResponse.Success(task.Id);
    }

    public async System.Threading.Tasks.Task<CompleteTaskResponse> CompleteAsync(CompleteTaskRequest request, CancellationToken ct)
    {
        var task = await _repo.GetByIdAsync(request.TaskId, ct);
        if (task == null) return new CompleteTaskResponse(false, "Not found");

        task.Status = DomainStatus.Completed;
        await _repo.UpdateAsync(task, ct);

        return new CompleteTaskResponse(true, "Completed");
    }

    public async System.Threading.Tasks.Task<TaskListResponse> GetAsync(GetTasksRequest request, CancellationToken ct)
    {
        var tasks = await _repo.GetByUserAsync(request.UserId.ToString(), ct);

        // FIX: Use Object Initializer {} instead of Constructor ()
        var dtos = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status.ToString(),
            DueDate = DateTime.UtcNow // Ensure TaskDto has a DueDate property
        }).ToList();

        return new TaskListResponse(dtos);
    }
}
