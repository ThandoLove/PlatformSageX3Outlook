using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.State
{
    public class UIState
    {
        private readonly IJSRuntime _js;

        public UIState(IJSRuntime js)
        {
            _js = js;
        }

        // THEME
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

        // ERROR / TOAST / MODAL EXAMPLES
        public string? ErrorMessage { get; set; }
        public bool ShowError { get; set; }

        public void SetError(string msg) { ErrorMessage = msg; ShowError = true; }
        public void ClearError() { ShowError = false; ErrorMessage = null; }

        // STATE CHANGE
        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}