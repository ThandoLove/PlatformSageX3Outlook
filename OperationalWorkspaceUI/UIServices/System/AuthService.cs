using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.Security;

namespace OperationalWorkspaceUI.UIServices.System;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _storage;
    private readonly AuthenticationStateProvider _authStateProvider;

    private const string TOKEN_KEY = "access_token";

    public AuthService(
        HttpClient http,
        ILocalStorageService storage,
        AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _storage = storage;
        _authStateProvider = authStateProvider;
    }

    // ======================================================
    // INIT SESSION
    // ======================================================

    public async Task InitializeAsync()
    {
        var token =
            await _storage.GetItemAsync<string>(TOKEN_KEY);

        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        // restore auth header
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // notify blazor auth state
        if (_authStateProvider
            is CustomAuthenticationStateProvider customAuth)
        {
            customAuth.NotifyUserAuthentication(token);
        }
    }

    // ======================================================
    // LOGIN
    // ======================================================

    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        var response =
            await _http.PostAsJsonAsync(
                "/api/v1/auth/login",
                dto);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json =
            await response.Content
                .ReadFromJsonAsync<JsonElement>();

        if (!json.TryGetProperty("data", out var data))
        {
            return false;
        }

        if (!data.TryGetProperty("token", out var tokenElement))
        {
            return false;
        }

        var token = tokenElement.GetString();

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        // save token
        await _storage.SetItemAsync(
            TOKEN_KEY,
            token);

        // set auth header
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        // notify blazor auth system
        if (_authStateProvider
            is CustomAuthenticationStateProvider customAuth)
        {
            customAuth.NotifyUserAuthentication(token);
        }

        return true;
    }

    // ======================================================
    // LOGOUT
    // ======================================================

    public async Task LogoutAsync()
    {
        await _storage.RemoveItemAsync(TOKEN_KEY);

        _http.DefaultRequestHeaders.Authorization = null;

        if (_authStateProvider
            is CustomAuthenticationStateProvider customAuth)
        {
            customAuth.NotifyUserLogout();
        }
    }

    // ======================================================
    // GET TOKEN
    // ======================================================

    public async Task<string?> GetTokenAsync()
    {
        return await _storage.GetItemAsync<string>(TOKEN_KEY);
    }
}