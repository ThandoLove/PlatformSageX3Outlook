using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspace.Domain.Enums;
// Use a global alias to bypass any namespace confusion
using Timer = System.Timers.Timer;

namespace OperationalWorkspaceUI.UIServices.ToastUIService
{
    public class ToastService : IToastUIService, IDisposable
    {
        public event Action<string, ToastLevel>? OnShow;
        public event Action? OnHide;

        // Use the alias here
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
                // Explicitly use System.Timers to avoid the "System" namespace conflict
                _countdown = new System.Timers.Timer(5000);
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
