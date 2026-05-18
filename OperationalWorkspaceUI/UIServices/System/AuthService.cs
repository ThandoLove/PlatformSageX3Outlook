using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using OperationalWorkspaceApplication.DTOs;
using Majorsoft.Blazor.Extensions.BrowserStorage;

namespace OperationalWorkspaceUI.UIServices.System;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private readonly ILocalStorageService _storage;

    private string? _accessToken;

    public AuthService(
        HttpClient http,
        NavigationManager nav,
        ILocalStorageService storage)
    {
        _http = http;
        _nav = nav;
        _storage = storage;
    }

    // ======================================================
    // INIT
    // ======================================================
    public async Task InitializeAsync()
    {
        var token = await _storage.GetItemAsync<string>("access_token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            await SetTokenAsync(token);
        }
    }

    // ======================================================
    // LOGIN
    // ======================================================
    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        var resp = await _http.PostAsJsonAsync("/api/v1/auth/login", dto);

        if (!resp.IsSuccessStatusCode)
            return false;

        var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();

        if (doc.TryGetProperty("data", out var data) &&
            data.TryGetProperty("token", out var tok))
        {
            var token = tok.GetString();

            if (!string.IsNullOrWhiteSpace(token))
            {
                await SetTokenAsync(token); // ✅ FIXED HERE
                return true;
            }
        }

        return false;
    }

    // ======================================================
    // SET TOKEN
    // ======================================================
    public async Task SetTokenAsync(string token)
    {
        _accessToken = token;

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await _storage.SetItemAsync("access_token", token);
    }

    // ======================================================
    // CLEAR TOKEN
    // ======================================================
    public async Task ClearTokenAsync()
    {
        _accessToken = null;

        _http.DefaultRequestHeaders.Authorization = null;

        await _storage.RemoveItemAsync("access_token");
    }

    public string? GetToken() => _accessToken;
}