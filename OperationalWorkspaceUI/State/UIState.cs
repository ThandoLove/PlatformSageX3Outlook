using Microsoft.JSInterop;

namespace OperationalWorkspaceUI.State
{
    public class UIState


    {
        public string UserName { get; set; } = "John Smith"; // Mock default
       



        private readonly IJSRuntime _js;

        public UIState(IJSRuntime js)
        {
            _js = js;
        }

        // =========================
        // THEME SYSTEM
        // =========================
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
            catch (InvalidOperationException)
            {
                /* Sink error if called during prerender */
            }
            NotifyStateChanged();
        }

        public async Task LoadThemeFromLocalStorage()
        {
            try
            {
                // SAFE CHECK: This will fail during prerendering, 
                // so we catch it to prevent the "Red Bar" crash.
                var theme = await _js.InvokeAsync<string>("localStorage.getItem", "theme");

                if (!string.IsNullOrEmpty(theme))
                {
                    CurrentTheme = theme;
                }
            }
            catch (InvalidOperationException)
            {
                // Prerendering phase: JS is not yet available.
                // We do nothing; the component will call this again 
                // in OnAfterRender once the browser is ready.
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

        // =========================
        // PAGE STATE
        // =========================
        public string CurrentPage { get; set; } = "Dashboard";
        public string CurrentPageTitle { get; set; } = "Dashboard";

        // =========================
        // MODAL
        // =========================
        public bool IsModalVisible { get; set; }
        public string CurrentModalTitle { get; set; } = string.Empty;
        public string CurrentModalContent { get; set; } = string.Empty;

        // =========================
        // NOTIFICATIONS
        // =========================
        public string ToastMessage { get; set; } = string.Empty;
        public string ToastType { get; set; } = "info";

        // =========================
        // ERROR HANDLING
        // =========================
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

        // =========================
        // LOADING
        // =========================
        public bool IsLoading { get; set; }

        // =========================
        // STATE CHANGE
        // =========================
        public event Action? OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
