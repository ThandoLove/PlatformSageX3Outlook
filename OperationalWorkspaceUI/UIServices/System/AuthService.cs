using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;
using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System
{
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
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav)); // Fixed: Added the missing nameof operator mapping
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _appState = appState ?? throw new ArgumentNullException(nameof(appState));
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
        // LOGIN (Fixed JSON property navigation tree)
        // ======================================================
        public async Task<bool> LoginAsync(LoginRequestDto dto)
        {
            if (dto == null) return false;

            try
            {
                var response = await _http.PostAsJsonAsync("/api/v1/auth/login", dto);

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                string? token = null;
                string? refreshToken = null;

                if (json.TryGetProperty("data", out var data))
                {
                    if (data.TryGetProperty("token", out var tokenElement))
                    {
                        token = tokenElement.GetString();
                    }

                    if (data.TryGetProperty("refreshToken", out var refreshElement))
                    {
                        refreshToken = refreshElement.GetString();
                    }
                }
                else if (json.TryGetProperty("token", out var rootTokenElement))
                {
                    token = rootTokenElement.GetString();
                }

                if (string.IsNullOrWhiteSpace(token))
                    return false;

                await _storage.SetItemAsync(AccessTokenKey, token);

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _storage.SetItemAsync(RefreshTokenKey, refreshToken);
                }

                SetAuthHeader(token);
                _appState.SetAuthentication(token);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ======================================================
        // REFRESH TOKEN
        // ======================================================
        public async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                var refreshToken = await _storage.GetItemAsync<string>(RefreshTokenKey);

                if (string.IsNullOrWhiteSpace(refreshToken))
                    return false;

                var response = await _http.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                string? newToken = null;

                if (json.TryGetProperty("data", out var data) && data.TryGetProperty("token", out var tokenElement))
                {
                    newToken = tokenElement.GetString();
                }
                else if (json.TryGetProperty("token", out var rootTokenElement))
                {
                    newToken = rootTokenElement.GetString();
                }

                if (string.IsNullOrWhiteSpace(newToken))
                    return false;

                await _storage.SetItemAsync(AccessTokenKey, newToken);
                SetAuthHeader(newToken);
                _appState.SetAuthentication(newToken);

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
        private void SetAuthHeader(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
