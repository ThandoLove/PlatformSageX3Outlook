using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System
{
    public class ModalService
    {
        // FIX: Add ? to make the events nullable
        public event Func<string, string, Task>? OnShow;
        public event Func<Task>? OnClose;

        public Task ShowModal(string title, string content)
        {
            // The ?. already handles the null check safely
            return OnShow?.Invoke(title, content) ?? Task.CompletedTask;
        }

        public Task CloseModal()
        {
            return OnClose?.Invoke() ?? Task.CompletedTask;
        }
    }
}
