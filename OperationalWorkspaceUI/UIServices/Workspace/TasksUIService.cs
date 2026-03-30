using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceUI.UIServices.Workspace;
using System.Net.Http.Json;

namespace OperationalWorkspaceUI.UIServices.Workspace;

public class TasksUIService
{
    private readonly HttpClient _http;
    private readonly WorkspaceState _workspaceState;
    private readonly List<TaskDto> _tasks = new();

    public TasksUIService(HttpClient http, WorkspaceState workspaceState)
    {
        _http = http;
        _workspaceState = workspaceState;
    }

    /// <summary>
    /// Fetches tasks from the API and updates the global WorkspaceState.
    /// Matches the call in Tasks.razor
    /// </summary>
    public async Task LoadTasksAsync()
    {
        try
        {
            // 1. Fetch data from your API endpoint
            var tasks = await _http.GetFromJsonAsync<List<TaskDto>>("api/tasks");

            // 2. Update the shared state container used by the Razor page
            _workspaceState.Tasks = tasks ?? new List<TaskDto>();
        }
        catch (Exception ex)
        {
            // Handle failure and ensure the UI doesn't crash
            Console.WriteLine($"Error loading tasks: {ex.Message}");
            _workspaceState.Tasks = new List<TaskDto>();
        }
    }

    public Task<List<TaskDto>> GetTasksAsync() => Task.FromResult(_workspaceState.Tasks?.ToList() ?? new List<TaskDto>());

    public async Task<bool> CreateTaskAsync(TaskDto task)
    {
        // Example: Post to API, then reload state
        var response = await _http.PostAsJsonAsync("api/tasks", task);
        if (response.IsSuccessStatusCode)
        {
            await LoadTasksAsync(); // Refresh the global list after creating
            return true;
        }
        return false;
    }
}
