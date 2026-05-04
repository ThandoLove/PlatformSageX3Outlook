
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using static OperationalWorkspaceApplication.Responses.TaskResponse;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface ITaskService
{
    Task<Responses.TaskResponse> CreateAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken);

    Task<CompleteTaskResponse> CompleteAsync(
        CompleteTaskRequest request,
        CancellationToken cancellationToken);

    Task<TaskListResponse> GetAsync(
        GetTasksRequest request,
        CancellationToken cancellationToken);
    Task<List<TaskDto>> GetTasksAssignedToAsync(string userId);
    Task<List<ApprovalDto>> GetPendingApprovalsAsync(string userId);
    Task<List<TaskDto>> GetAllTasksAsync();
    Task<List<ApprovalDto>> GetAllPendingApprovalsAsync();

    Task<TaskResponse> DelegateAsync(DelegateTaskRequest request, CancellationToken ct);

}