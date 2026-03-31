using Microsoft.JSInterop;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OperationalWorkspaceApplication.DTOs;
using Microsoft.AspNetCore.Components;

namespace OperationalWorkspaceUI.UIServices.System;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private string? _accessToken;

    public AuthService(HttpClient http, NavigationManager nav)
    {
        _http = http;
        _nav = nav;
    }

    public void SetToken(string token)
    {
        _accessToken = token;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearToken()
    {
        _accessToken = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        var resp = await _http.PostAsJsonAsync("/api/v1/auth/login", dto);
        if (!resp.IsSuccessStatusCode) return false;
        var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
        if (doc.TryGetProperty("data", out var data) && data.TryGetProperty("token", out var tok))
        {
            var token = tok.GetString();
            if (!string.IsNullOrEmpty(token))
            {
                SetToken(token);
                return true;
            }
        }
        return false;
    }
}