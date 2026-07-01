using Microsoft.JSInterop;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.Interfaces;

namespace OperationalWorkspaceUI.State
{
    public class UIState : IDisposable
    {
        private readonly IJSRuntime _js;

        public UIState(IJSRuntime js)
        {
            _js = js;
        }

        // ======================================================
        // AUTHENTICATION & ROLE MANAGEMENT ENGINE (🚀 OPEN SETTERS)
        // ======================================================
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public bool IsAdminEnvironment { get; set; } = false;

        // FIXED: Changed from read-only lambda expression string evaluation to mutable auto-property
        public bool IsAuthenticated { get; set; } = false;
        // ✅ FIX: State guard property blocks distorted rendering during the initial async token check
        public bool IsCheckingAuth { get; set; } = true;


        /// <summary>
        /// Synchronizes the centralized UI layout directly from an identity claims principal payload.
        /// </summary>
        public void SynchronizeUserSession(ClaimsPrincipal principal)
        {
            if (principal?.Identity?.IsAuthenticated == true)
            {
                // ✅ FIX 7: Order changed to prioritize human-readable ClaimTypes.Name over email values
                UserName = principal.FindFirst(ClaimTypes.Name)?.Value
                           ?? principal.FindFirst("name")?.Value
                           ?? principal.FindFirst("unique_name")?.Value
                           ?? "Operator User";

                // ✅ FIX 7: Changed fallback string assignment component to empty string safety defaults
                UserEmail = principal.FindFirst(ClaimTypes.Email)?.Value
                            ?? principal.FindFirst("email")?.Value
                            ?? string.Empty;

                UserRole = principal.FindFirst(ClaimTypes.Role)?.Value
                           ?? principal.FindFirst("role")?.Value
                           ?? "Employee";

                IsAdminEnvironment = UserRole.Equals("Administrator", StringComparison.OrdinalIgnoreCase) ||
                                     UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                IsAuthenticated = true;
            }
            else
            {
                ClearUser();
            }
            NotifyStateChanged();
        }

        /// <summary>
        /// Manual override fallback method to directly assign values if claims parameters aren't supplied.
        /// </summary>
        public void SetUser(string userName, string userEmail)
        {
            UserName = userName;
            UserEmail = userEmail;
            UserRole = "Employee";
            IsAdminEnvironment = false;
            IsAuthenticated = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Manual override fallback method to directly assign values with administrative privileges.
        /// </summary>
        public void SetAdminUser(string userName, string userEmail)
        {
            UserName = userName;
            UserEmail = userEmail;
            UserRole = "Administrator";
            IsAdminEnvironment = true;
            IsAuthenticated = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Clears the user session data and role environments on logout.
        /// </summary>
        public void ClearUser()
        {
            UserName = string.Empty;
            UserEmail = string.Empty;
            UserRole = string.Empty;
            IsAdminEnvironment = false;
            IsAuthenticated = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// Utility to force trigger mock/dummy data for local UI development and testing.
        /// </summary>
        public void LoadDummyUser()
        {
            UserName = "John Smith";
            UserEmail = "john.smith@company.com";
            UserRole = "Employee";
            IsAdminEnvironment = false;
            IsAuthenticated = true;
            NotifyStateChanged();
        }

        // ======================================================
        // 🌐 SAGE X3 HEALTH STATUS STATE MACHINE
        // ======================================================
        // FIXED: Shifted from private set to an open public setter accessor
        public bool IsSageConnected { get; set; } = false;

        /// <summary>
        /// Asynchronously verifies network pipeline connectivity using higher-level abstractions.
        /// </summary>
        public async Task CheckSageX3ConnectionHealthAsync(ISageX3Client sageClient)
        {
            try
            {
                var count = await sageClient.GetActivePartnersCountAsync();
                IsSageConnected = count > 0;
            }
            catch
            {
                IsSageConnected = false;
            }
            finally
            {
                NotifyStateChanged();
            }
        }

        // ======================================================
        // THEME SYSTEM
        // ======================================================
        public string CurrentTheme { get; private set; } = "light-mode";
        public bool IsGlassMode => CurrentTheme == "glass-mode";
        public bool IsDarkMode => CurrentTheme == "dark-mode";

        public async Task SetTheme(string theme)
        {
            CurrentTheme = theme;
            try
            {
                await _js.InvokeVoidAsync("localStorage.setItem", "theme", theme);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is JSException)
            {
            }
            NotifyStateChanged();
        }

        public async Task LoadThemeFromLocalStorage()
        {
            try
            {
                var theme = await _js.InvokeAsync<string>("localStorage.getItem", "theme");
                if (!string.IsNullOrEmpty(theme))
                {
                    CurrentTheme = theme;
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is JSException)
            {
            }
            finally
            {
                NotifyStateChanged();
            }
        }

        public async Task ToggleGlassMode(bool value)
        {
            await SetTheme(value ? "glass-mode" : "light-mode");
        }

        // ======================================================
        // PAGE STATE
        // ======================================================
        public string CurrentPage { get; set; } = "Dashboard";
        public string CurrentPageTitle { get; set; } = "Dashboard";

        // ======================================================
        // MODAL
        // ======================================================
        public bool IsModalVisible { get; set; }
        public string CurrentModalTitle { get; set; } = string.Empty;
        public string CurrentModalContent { get; set; } = string.Empty;

        // ======================================================
        // NOTIFICATIONS
        // ======================================================
        public string ToastMessage { get; set; } = string.Empty;
        public string ToastType { get; set; } = "info";

        // ======================================================
        // ERROR HANDLING
        // ======================================================
        public string? ErrorMessage { get; set; }
        public bool ShowError { get; set; }

        public void SetError(string msg)
        {
            ErrorMessage = msg;
            ShowError = true;
            NotifyStateChanged();
        }

        public void ClearError()
        {
            ShowError = false;
            ErrorMessage = null;
            NotifyStateChanged();
        }

        // ======================================================
        // LOADING
        // ======================================================
        public bool IsLoading { get; set; }

        // ======================================================
        // EVENTS
        // ======================================================
        public event Action? OnChange;

        public void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        // Enforces proper IDisposable lifecycle garbage compilation cleanup mechanics
        public void Dispose()
        {
            OnChange = null;
        }
    }
}
