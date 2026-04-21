using OperationalWorkspace.Domain.Enums;


namespace OperationalWorkspaceUI.UIServices.ToastUIService
{

    public interface IToastUIService
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

}
