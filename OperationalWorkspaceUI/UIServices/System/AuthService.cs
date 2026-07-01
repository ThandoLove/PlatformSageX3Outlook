using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.Security;
using OperationalWorkspaceUI.State;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System;

public class AuthService
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string ActiveSageFolderKey = "active_sage_folder";

    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private readonly ILocalStorageService _storage;
    private readonly AppStateContainer _appState;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly UIState _uiState;
    // FIX 2: Added structural field tracking for the concrete security state provider
    private readonly CustomAuthenticationStateProvider _authProvider;

    public AuthService(
        HttpClient http,
        NavigationManager nav,
        ILocalStorageService storage,
        AppStateContainer appState,
        IWebHostEnvironment env,
        IConfiguration config,
        UIState uiState,
        CustomAuthenticationStateProvider authProvider) // FIX 2: Injected the synchronized state provider token
    {
        _http = http;
        _nav = nav;
        _storage = storage;
        _appState = appState;
        _env = env;
        _config = config;
        _uiState = uiState;
        _authProvider = authProvider; // FIX 2: Bound to internal context instance
    }

    // STUB REQUIRED BY MAINLAYOUT
    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    // FIX 5: Hardened logout method ensuring all storage scopes and state notifications reset
    public async Task LogoutAsync()
    {
        // 1. Wipe persistent cookies and browser local storage contexts cleanly
        await _storage.RemoveItemAsync(AccessTokenKey);
        await _storage.RemoveItemAsync(RefreshTokenKey);
        await _storage.RemoveItemAsync(ActiveSageFolderKey);

        // 2. Clear state providers to automatically collapse admin visibility sidebars
        _appState.ClearAuthentication();
        _uiState.ClearUser();

        // 3. Broadcast state mutation down to Blazor cascading rendering tree blocks
        _authProvider.NotifyUserLogout();

        // 4. ✅ FIX: Route to the site root index instead of a broken, non-existent "/login" subpath
        _nav.NavigateTo("/", forceLoad: false);
    }


    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        if (dto == null) return false;

        var isDemoFallback = _config["BlazorExecutionMode"] == "DevelopmentDemo";

        if (_env.IsDevelopment() || isDemoFallback)
        {
            // ✅ DYNAMIC LOCAL GENERATION RULE: Pull exactly what the user typed on the form fields
            var typedInputName = dto.Username;

            // Extract a display nickname fallback if an full email was supplied
            var splitUserPrefix = typedInputName.Contains("@") ? typedInputName.Split('@')[0] : typedInputName;
            var formattedDisplayName = global::System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(splitUserPrefix.Replace(".", " "));

            // Automatically check role boundaries using your case-insensitive logic
            string computedRole = typedInputName.Contains("admin", StringComparison.OrdinalIgnoreCase)
                ? "Administrator"
                : "Employee";

            // FIX 6: Enterprise dynamic raw string token serialization payload layout block
            string dummyPayloadJson = $$"""
            {
                "name":"{{formattedDisplayName}}",
                "unique_name":"{{formattedDisplayName}}",
                "email":"{{typedInputName}}",
                "role":"{{computedRole}}",
                "exp":4127131100,
                "permission":"orders.view",
                "is_sage_user":true,
                "sage_env":"SEED"
            }
            """;

            string encodedPayload = Convert.ToBase64String(global::System.Text.Encoding.UTF8.GetBytes(dummyPayloadJson))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');

            string simulatedJwt = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{encodedPayload}.demo_signature_hash";

            await _storage.SetItemAsync(AccessTokenKey, simulatedJwt);
            await _storage.SetItemAsync(RefreshTokenKey, "demo_refresh_token");
            await _storage.SetItemAsync(ActiveSageFolderKey, "SEED");

            SetAuthHeader(simulatedJwt);
            _appState.SetAuthentication(simulatedJwt);

            // Dynamically assign local variables instead of stale hardcoded properties
            _uiState.UserName = formattedDisplayName;
            _uiState.UserEmail = typedInputName;
            _uiState.IsAuthenticated = true;
            _uiState.IsSageConnected = true;
            _uiState.NotifyStateChanged();

            // FIX 4: Notify Blazor security context state layers after successful demo simulation
            _authProvider.NotifyUserAuthentication(simulatedJwt);

            _appState.SetActiveSageEndpoint("SEED");
            _nav.NavigateTo("/");
            return true;
        }

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

            _uiState.UserName = dto.Username;
            _uiState.UserEmail = dto.Username;
            _uiState.IsAuthenticated = true;
            _uiState.IsSageConnected = true;
            _uiState.NotifyStateChanged();

            // FIX 4: Notify Blazor security context state layers after production API auth confirmation
            _authProvider.NotifyUserAuthentication(token);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void SetAuthHeader(string token)
    {
        if (_http.DefaultRequestHeaders.Contains("Authorization"))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
        }
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
}
