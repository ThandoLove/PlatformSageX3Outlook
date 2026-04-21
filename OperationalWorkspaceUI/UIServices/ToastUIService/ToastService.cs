using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspace.Domain.Enums;
// Fix 1: Use global:: to force the compiler to find the real .NET System.Timers
using Timer = global::System.Timers.Timer;

namespace OperationalWorkspaceUI.UIServices.ToastUIService
{
    public class ToastService : IToastUIService, IDisposable
    {
        public event Action<string, ToastLevel>? OnShow;
        public event Action? OnHide;

        private Timer? _countdown;

        public void ShowToast(string message, ToastLevel level = ToastLevel.Info)
        {
            OnShow?.Invoke(message, level);
            StartCountdown();
        }

        public void ShowSuccess(string message) => ShowToast(message, ToastLevel.Success);
        public void ShowError(string message) => ShowToast(message, ToastLevel.Error);
        public void ShowInfo(string message) => ShowToast(message, ToastLevel.Info);

        private void StartCountdown()
        {
            if (_countdown == null)
            {
                // Fix 2: Again, use global:: here to be absolutely safe
                _countdown = new global::System.Timers.Timer(5000);
                _countdown.Elapsed += (s, e) => OnHide?.Invoke();
                _countdown.AutoReset = false;
            }

            if (_countdown.Enabled)
            {
                _countdown.Stop();
            }
            _countdown.Start();
        }

        public void Dispose()
        {
            if (_countdown != null)
            {
                _countdown.Stop();
                _countdown.Dispose();
            }
        }
    }
}
