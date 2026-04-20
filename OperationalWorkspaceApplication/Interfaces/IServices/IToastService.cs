using OperationalWorkspace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface IToastService
{
    // Events for the UI component to listen to
    event Action<string, ToastLevel>? OnShow;
    event Action? OnHide;

    // Methods to call from your logic
    void ShowToast(string message, ToastLevel level = ToastLevel.Info);
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowInfo(string message);
}
