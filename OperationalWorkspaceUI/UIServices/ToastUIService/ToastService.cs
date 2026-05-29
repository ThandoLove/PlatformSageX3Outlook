using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspace.Domain.Enums;
using System;
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
            // 🚀 STABILIZED TIMER RESET MECHANISM:
            // If a timer already exists from a previous alert, completely kill and stop it.
            // This resets the countdown buffer cleanly so overlapping click alerts don't stack up or freeze.
            if (_countdown != null)
            {
                _countdown.Stop();
                _countdown.Dispose();
                _countdown = null;
            }

            // Create a completely clean 3000ms (3 seconds) window. 
            // 5 seconds feels way too long and makes the user think the UI is locked up.
            _countdown = new global::System.Timers.Timer(3000);

            _countdown.Elapsed += (s, e) =>
            {
                // Ensure the timer explicitly cleans up after it finishes executing its hide invoke
                _countdown?.Stop();
                OnHide?.Invoke();
            };

            _countdown.AutoReset = false;
            _countdown.Start();
        }

        public void Dispose()
        {
            if (_countdown != null)
            {
                _countdown.Stop();
                _countdown.Dispose();
                _countdown = null;
            }
        }
    }
}
