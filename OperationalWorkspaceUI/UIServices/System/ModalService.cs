
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System
{
    public class ModalService
    {
        public event Func<string, string, Task> OnShow;
        public event Func<Task> OnClose;

        public Task ShowModal(string title, string content)
        {
            return OnShow?.Invoke(title, content) ?? Task.CompletedTask;
        }

        public Task CloseModal()
        {
            return OnClose?.Invoke() ?? Task.CompletedTask;
        }
    }
}