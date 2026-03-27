using Microsoft.JSInterop;

namespace OperationalWorkspaceUI.State
{
    public class UIState
    {
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
            await _js.InvokeVoidAsync("localStorage.setItem", "theme", theme);
            NotifyStateChanged();
        }

        public async Task LoadThemeFromLocalStorage()
        {
            var theme = await _js.InvokeAsync<string>("localStorage.getItem", "theme");
            if (!string.IsNullOrEmpty(theme))
                CurrentTheme = theme;

            NotifyStateChanged();
        }

        // 👇 ADD THIS (for toggle compatibility)
        public async Task ToggleGlassMode(bool value)
        {
            await SetTheme(value ? "glass-mode" : "light-mode");
        }

        // =========================
        // PAGE STATE (YOU LOST THIS)
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