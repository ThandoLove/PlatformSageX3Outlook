// CODE START
using System.Net.Http.Json;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceUI.Models.Forms;

namespace OperationalWorkspaceUI.UIServices.Actions;

public class QuickActionUIService
{
    private readonly HttpClient _http;

    public QuickActionUIService(HttpClient http)
    {
        _http = http;
    }

    // ---------------- NEW METHODS REQUIRED BY UI ----------------

    public async Task CreateClientFromEmailAsync()
    {
        // Call the API endpoint that creates a client from the selected email
        var response = await _http.PostAsync("api/clients/from-email", null);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to create client from email");
    }

    public async Task OpenClientSearchAsync()
    {
        // UI-only logic: open a modal/search window
        await Task.CompletedTask;
    }

    // ---------------- EXISTING METHODS ----------------

    public async Task CreateTaskFromEmailAsync(EmailInsightDto email)
    {
        // Use the { } syntax because CreateTaskRequest is a class, not a positional record
        var request = new CreateTaskRequest
        {
            Title = $"Follow-up: {email.Subject}",
            Description = email.Message,
            AssignedTo = email.AssignedUserId,
            DueDate = DateTime.UtcNow.AddDays(3),
            CreatedBy = "System", // Added because your class requires it
            Priority = 1         // Added because your class requires it
        };

        var response = await _http.PostAsJsonAsync("api/tasks", request);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to create task from email");

        await LogActivity("Task Created from Email", email.Subject);
    }

    public async Task AttachEmailAsync(object model)
    {
        var response = await _http.PostAsJsonAsync("api/email/attach", model);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to attach email");

        await LogActivity("Email Attached", "Email linked");
    }

    public async Task SendKnowledgeAsync(object model)
    {
        var response = await _http.PostAsJsonAsync("api/knowledge/send", model);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to send knowledge");

        await LogActivity("Knowledge Sent", "Sent to client");
    }

    public async Task<List<KnowledgeDto>> SearchKnowledgeAsync(string query)
    {
        return await _http.GetFromJsonAsync<List<KnowledgeDto>>($"api/knowledge/search?q={query}")
               ?? new List<KnowledgeDto>();
    }

    private async Task LogActivity(string action, string description)
    {
        await _http.PostAsJsonAsync("api/activity/log", new ActivityDto
        {
            Action = action,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });
    }
}
// CODE END