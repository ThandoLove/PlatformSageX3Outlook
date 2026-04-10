// CODE START

using System.Net.Http.Json;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.UIServices.EmailService;

public class EmailContextUIService
{
    private readonly HttpClient _http;

    public EmailContextUIService(HttpClient http)
    {
        _http = http;
    }

    public async Task LoadEmailContextAsync(string emailId, EmailContextState state, WorkspaceState workspaceState)
    {
        // 1. Email
        state.CurrentEmail =
            await _http.GetFromJsonAsync<EmailInsightDto>($"api/email/{emailId}");

        // 2. Orders
        var orders =
            await _http.GetFromJsonAsync<List<OrderDto>>($"api/email/{emailId}/orders");

        state.LinkedOrders = orders ?? new List<OrderDto>();

        // 3. Tasks
        state.LinkedTasks =
            await _http.GetFromJsonAsync<List<TaskDto>>($"api/email/{emailId}/tasks")
            ?? new List<TaskDto>();

        // 4. Match client
        if (state.CurrentEmail != null && workspaceState.Clients != null)
        {
            state.MatchedClient = workspaceState.Clients
                .FirstOrDefault(c => c.Email == state.CurrentEmail.From);
        }
    }
}

// CODE END