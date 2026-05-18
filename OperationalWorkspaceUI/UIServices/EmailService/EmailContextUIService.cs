using System.Net.Http.Json;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.UIServices.EmailService;

public class EmailContextUIService
{
    private readonly HttpClient _http;

    public EmailContextUIService(HttpClient http)
    {
        _http = http;
    }

    // -------------------------------
    // SINGLE RESPONSIBILITY: LOAD CONTEXT
    // -------------------------------
    public async Task<EmailInsightDto?> LoadEmailContextAsync(string emailId)
    {
        return await _http.GetFromJsonAsync<EmailInsightDto>(
            $"api/email/{emailId}/context");
    }
}