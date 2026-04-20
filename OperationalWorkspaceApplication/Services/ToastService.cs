using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.Interfaces.IServices;



namespace OperationalWorkspaceApplication.Services;
public class ToastService : IToastService, IDisposable
{
    public event Action<string, ToastLevel>? OnShow;
    public event Action? OnHide;
    private System.Timers.Timer? _countdown;

    public void ShowToast(string message, ToastLevel level = ToastLevel.Info)
    {
        OnShow?.Invoke(message, level);
        StartCountdown();
    }

    // Helper methods for cleaner "fucking logic" in your components
    public void ShowSuccess(string message) => ShowToast(message, ToastLevel.Success);
    public void ShowError(string message) => ShowToast(message, ToastLevel.Error);
    public void ShowInfo(string message) => ShowToast(message, ToastLevel.Info);

    private void StartCountdown()
    {
        if (_countdown == null)
        {
            _countdown = new System.Timers.Timer(5000); // 5 seconds duration
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
        _countdown?.Dispose();
    }
}
