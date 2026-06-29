using Microsoft.JSInterop;
using System;
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
        // AUTHENTICATION & USER STATE (🚀 FIXED FOR MULTI-PANEL SYNC)
        // ======================================================

        // In production, default this to empty. The TopBar component handles the fallback to "John Smith".
        public string UserName { get; private set; } = string.Empty;
        public string UserEmail { get; private set; } = string.Empty; // 🚀 FIXED: Exposed with public read access
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserName);

        /// <summary>
        /// Updates the state with the production authenticated user.
        /// </summary>
        public void SetUser(string userName, string userEmail)
        {
            UserName = userName;
            UserEmail = userEmail; // 🚀 FIXED: Hydrates both fields simultaneously on successful login
            NotifyStateChanged();
        }

        /// <summary>
        /// Clears the user session data on logout.
        /// </summary>
        public void ClearUser()
        {
            UserName = string.Empty;
            UserEmail = string.Empty;
            NotifyStateChanged();
        }

        /// <summary>
        /// Utility to force trigger mock/dummy data for local UI development and testing.
        /// </summary>
        public void LoadDummyUser()
        {
            UserName = "John Smith";
            UserEmail = "john.smith@company.com";
            NotifyStateChanged();
        }

        // ======================================================
        // 🌐 SAGE X3 HEALTH STATUS STATE MACHINE (DECOUPLED FROM UI)
        // ======================================================
        public bool IsSageConnected { get; private set; } = false;

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
                // Caught safely during Prerendering or if localStorage is restricted
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
                // Caught safely during Prerendering or if localStorage is restricted
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

        // ======================================================
        // MEMORY CLEANUP
        // ======================================================
        public void Dispose()
        {
            OnChange = null;
        }
    }
}
