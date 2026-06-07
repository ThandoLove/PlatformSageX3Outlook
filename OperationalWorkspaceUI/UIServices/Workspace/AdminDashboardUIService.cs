using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.Workspace;

public class AdminDashboardUIService
{
    private readonly HttpClient _httpClient;

    public AdminDashboardUIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // 1. LIVE PIPELINE: CREATE USER FORM CONTROLLER ACTION
    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/AdminDashboard/create-user", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            // 🔴 FIX: Prefix with global:: to bypass your local custom folder namespace collision
            global::System.Diagnostics.Debug.WriteLine($"User registration transit error: {ex.Message}");
            return false;
        }
    }

    // 2. LIVE PIPELINE: EXPORT REPORT FILE BYTES DOWNLOAD STREAM
    public async Task<byte[]> DownloadReportPdfAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/AdminDashboard/export-report");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            // 🔴 FIX: Prefix with global:: to bypass your local custom folder namespace collision
            global::System.Diagnostics.Debug.WriteLine($"Report generation download fault: {ex.Message}");
            return Array.Empty<byte>();
        }
    }

    public async Task<List<ActivityDto>> GetRecentActivityAsync()
    {
        await Task.Delay(100);
        return new List<ActivityDto>();
    }

    public async Task<int> GetTotalUsersAsync()
    {
        await Task.Delay(100);
        return 1284;
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        await Task.Delay(50);
        return new UserDto
        {
            Name = "System Administrator",
            Role = "Global Administrator",
            Environment = "Production"
        };
    }

    public async Task<List<AuditLogDto>> GetRecentLogsAsync()
    {
        await Task.Delay(50);
        return new List<AuditLogDto>
        {
            new() { Action = "User Login", User = "admin@workspace.com", Timestamp = DateTime.Now.AddMinutes(-5) },
            new() { Action = "ERP Config Update", User = "manager@workspace.com", Timestamp = DateTime.Now.AddHours(-2) }
        };
    }
}
