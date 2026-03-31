using OperationalWorkspaceApplication.DTOs;
using System.Net.Http.Json;

namespace OperationalWorkspaceUI.UIServices.EmailService;

public class EmailSyncService
{
    private readonly HttpClient _http;

    public EmailSyncService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> SendEmailInsightAsync(EmailInsightDto dto)
    {
        var resp = await _http.PostAsJsonAsync("/api/v1/email/send", dto);
        return resp.IsSuccessStatusCode;
    }
}
