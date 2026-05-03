using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceUI.State;
using System.Net.Http.Json;

namespace OperationalWorkspaceUI.UIServices.Workspace;

public class TasksUIService
{
    private readonly HttpClient _http;
    private readonly WorkspaceState _workspaceState;

    public TasksUIService(HttpClient http, WorkspaceState workspaceState)
    {
        _http = http;
        _workspaceState = workspaceState;
    }

    public async Task LoadTasksAsync()
    {
        try
        {
            var tasks = await _http.GetFromJsonAsync<List<TaskDto>>("api/tasks");
            _workspaceState.Tasks = tasks ?? new List<TaskDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
            _workspaceState.Tasks = new List<TaskDto>();
        }
    }

    public Task<List<TaskDto>> GetTasksAsync()
        => Task.FromResult(_workspaceState.Tasks?.ToList() ?? new List<TaskDto>());

    // ✅ FIXED METHOD (THIS IS THE ONLY IMPORTANT CHANGE)
    public async Task<bool> CreateTaskAsync(CreateTaskRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/tasks", request);

        if (response.IsSuccessStatusCode)
        {
            await LoadTasksAsync(); // refresh UI
            return true;
        }

        return false;
    }



}