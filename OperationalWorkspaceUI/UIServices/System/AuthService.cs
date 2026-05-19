using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;
using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.DTOs;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OperationalWorkspaceUI.UIServices.System;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private readonly ILocalStorageService _storage;
    private readonly AppStateContainer _appState;

    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public AuthService(
        HttpClient http,
        NavigationManager nav,
        ILocalStorageService storage,
        AppStateContainer appState)
    {
        _http = http;
        _nav = nav;
        _storage = storage;
        _appState = appState;
    }

    // ======================================================
    // INIT SESSION (RESTORE LOGIN ON STARTUP)
    // ======================================================

    public async Task InitializeAsync()
    {
        var token = await _storage.GetItemAsync<string>(AccessTokenKey);

        if (string.IsNullOrWhiteSpace(token))
            return;

        SetAuthHeader(token);
        _appState.SetAuthentication(token);
    }

    // ======================================================
    // LOGIN
    // ======================================================

    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        var response = await _http.PostAsJsonAsync(
            "/api/v1/auth/login",
            dto);

        if (!response.IsSuccessStatusCode)
            return false;

        // SAFE: works with ANY backend shape (no AuthResultDto needed)
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (!json.TryGetProperty("accessToken", out var tokenElement) &&
            !json.TryGetProperty("token", out tokenElement) &&
            !(json.TryGetProperty("data", out var data) &&
              data.TryGetProperty("token", out tokenElement)))
        {
            return false;
        }

        var token = tokenElement.GetString();

        if (string.IsNullOrWhiteSpace(token))
            return false;

        await StoreSessionAsync(token);

        return true;
    }

    // ======================================================
    // REFRESH TOKEN
    // ======================================================

    public async Task<bool> TryRefreshTokenAsync()
    {
        try
        {
            var refreshToken =
                await _storage.GetItemAsync<string>(RefreshTokenKey);

            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var response = await _http.PostAsJsonAsync(
                "/api/v1/auth/refresh",
                new { refreshToken });

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!json.TryGetProperty("accessToken", out var tokenElement) &&
                !(json.TryGetProperty("data", out var data) &&
                  data.TryGetProperty("accessToken", out tokenElement)))
            {
                return false;
            }

            var newToken = tokenElement.GetString();

            if (string.IsNullOrWhiteSpace(newToken))
                return false;

            await StoreSessionAsync(newToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    // ======================================================
    // LOGOUT
    // ======================================================

    public async Task LogoutAsync()
    {
        _http.DefaultRequestHeaders.Authorization = null;

        _appState.ClearAuthentication();

        await _storage.RemoveItemAsync(AccessTokenKey);
        await _storage.RemoveItemAsync(RefreshTokenKey);

        _nav.NavigateTo("/login");
    }

    // ======================================================
    // INTERNAL HELPERS
    // ======================================================

    private async Task StoreSessionAsync(string token)
    {
        await _storage.SetItemAsync(AccessTokenKey, token);

        SetAuthHeader(token);

        _appState.SetAuthentication(token);
    }

    private void SetAuthHeader(string token)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}