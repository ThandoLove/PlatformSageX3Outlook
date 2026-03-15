
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
}