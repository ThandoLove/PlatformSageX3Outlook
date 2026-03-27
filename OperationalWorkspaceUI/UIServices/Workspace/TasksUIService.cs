namespace OperationalWorkspaceUI.UIServices.Workspace;

using OperationalWorkspaceApplication.DTOs;

public class TasksUIService
{
    private readonly List<TaskDto> _tasks = new();

    public Task<List<TaskDto>> GetTasksAsync() => Task.FromResult(_tasks);

    // FIXED: Renamed to match the Razor component and accepts TaskDto
    public Task<bool> CreateTaskAsync(TaskDto task)
    {
        _tasks.Add(task);
        return Task.FromResult(true);
    }
}
