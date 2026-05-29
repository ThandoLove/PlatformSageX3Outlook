using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string ActiveSageFolderKey = "active_sage_folder";

        public AuthService(
            HttpClient http,
            NavigationManager nav,
            ILocalStorageService storage,
            AppStateContainer appState,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _appState = appState ?? throw new ArgumentNullException(nameof(appState));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
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

            // Restores the exact active Sage folder context on startup initialization
            var savedFolder = await _storage.GetItemAsync<string>(ActiveSageFolderKey);
            if (!string.IsNullOrWhiteSpace(savedFolder))
            {
                _appState.SetActiveSageEndpoint(savedFolder);
            }
        }

        // ======================================================
        // MULTI-ENVIRONMENT LOGIN MANAGEMENT
        // ======================================================
        public async Task<bool> LoginAsync(LoginRequestDto dto)
        {
            if (dto == null) return false;

            var isDemoFallback = _config["BlazorExecutionMode"] == "DevelopmentDemo";

            // --------------------------------------------------
            // BRANCH 1: LIVE DEMO LOCAL BYPASS (Development Only)
            // --------------------------------------------------
            if (_env.IsDevelopment() || isDemoFallback)
            {
                string dummyPayloadJson = "{\"unique_name\":\"operator@test.com\",\"role\":\"Admin\",\"permission\":\"orders.view\",\"is_sage_user\":true,\"sage_env\":\"SEED\"}";

                string encodedPayload = Convert.ToBase64String(global::System.Text.Encoding.UTF8.GetBytes(dummyPayloadJson))
                    .Replace('+', '-').Replace('/', '_').TrimEnd('=');

                string simulatedJwt = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{encodedPayload}.demo_signature_hash";

                await _storage.SetItemAsync(AccessTokenKey, simulatedJwt);
                await _storage.SetItemAsync(RefreshTokenKey, "demo_refresh_token");
                await _storage.SetItemAsync(ActiveSageFolderKey, "SEED");

                SetAuthHeader(simulatedJwt);
                _appState.SetAuthentication(simulatedJwt);

                // UNIFICATION HOOK: Update your native local AppStateContainer environment parameter
                _appState.SetActiveSageEndpoint("SEED");

                _nav.NavigateTo("/");
                return true;
            }

            // --------------------------------------------------
            // BRANCH 2: SECURE ENTERPRISE NETWORK CALL (Production Only)
            // --------------------------------------------------
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

                // PRODUCTION ENFORCEMENT PARSING CHECK LINKAGE
                /*
                ====================================================================================
                █████████████████████ PRODUCTION EXPLICIT GOVERNANCE IDENTITY ENFORCEMENT █████████████████████
                ====================================================================================
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

                var isSageUser = jsonToken?.Payload.ContainsKey("is_sage_user") == true 
                    && bool.Parse(jsonToken.Payload["is_sage_user"].ToString() ?? "false");

                if (!isSageUser)
                {
                    return false;
                }

                string sageFolder = jsonToken?.Payload["sage_env"]?.ToString() ?? "PROD";
                await _storage.SetItemAsync(ActiveSageFolderKey, sageFolder);
                
                // UNIFICATION HOOK: Update your native local AppStateContainer environment parameter
                _appState.SetActiveSageEndpoint(sageFolder);
                ====================================================================================
                */

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
            var isDemoFallback = _config["BlazorExecutionMode"] == "DevelopmentDemo";
            if (_env.IsDevelopment() || isDemoFallback) return true;

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
            await _storage.RemoveItemAsync(ActiveSageFolderKey);

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
