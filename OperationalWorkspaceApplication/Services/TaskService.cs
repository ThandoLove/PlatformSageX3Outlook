using System.Linq;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using DomainTask = OperationalWorkspace.Domain.Entities.TaskEntity;
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
        var task = new DomainTask
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

        var dtos = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            DueDate = DateTime.UtcNow,
            Completed = t.Status == DomainStatus.Completed
        }).ToList();

        return new TaskListResponse(dtos);
    }

    public async System.Threading.Tasks.Task<List<TaskDto>> GetTasksAssignedToAsync(string userId)
    {
        var tasks = await _repo.GetByUserAsync(userId, CancellationToken.None);

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            DueDate = DateTime.UtcNow,
            Completed = t.Status == DomainStatus.Completed
        }).ToList();
    }

    public async System.Threading.Tasks.Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId)
    {
        var approvals = await _repo.GetPendingApprovalsAsync(userId, CancellationToken.None);
        return approvals.Select(a => new ApprovalDto { }).ToList();
    }

    public async System.Threading.Tasks.Task<List<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _repo.GetAllAsync(CancellationToken.None);

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            DueDate = DateTime.UtcNow,
            Completed = t.Status == DomainStatus.Completed
        }).ToList();
    }

    public async System.Threading.Tasks.Task<List<ApprovalDto>> GetAllPendingApprovalsAsync()
    {
        var approvals = await _repo.GetPendingApprovalsAsync(null, CancellationToken.None);
        return approvals.Select(a => new ApprovalDto { }).ToList();
    }
}