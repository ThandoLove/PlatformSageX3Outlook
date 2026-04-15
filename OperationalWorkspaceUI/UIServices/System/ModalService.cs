using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System
{
    /// <summary>
    /// Global service to manage the display of modal windows across the application.
    /// Handles everything from simple alerts to complex Sage X3 data entry forms.
    /// </summary>
    public class ModalService
    {
        /// <summary>
        /// Event triggered when a modal is requested. 
        /// Passes the title and the Blazor UI content (RenderFragment).
        /// </summary>
        public event Func<string, RenderFragment, Task>? OnShow;

        /// <summary>
        /// Event triggered when the current modal should be hidden.
        /// </summary>
        public event Func<Task>? OnClose;

        /// <summary>
        /// Triggers the display of a modal.
        /// </summary>
        /// <param name="title">The header text of the modal.</param>
        /// <param name="content">The Blazor component or HTML content to display.</param>
        public Task ShowModal(string title, RenderFragment content)
        {
            // The ?. ensures that if no component (like MainLayout) is listening, the app doesn't crash.
            return OnShow?.Invoke(title, content) ?? Task.CompletedTask;
        }

        /// <summary>
        /// Triggers the closing of the active modal.
        /// </summary>
        public Task CloseModal()
        {
            return OnClose?.Invoke() ?? Task.CompletedTask;
        }
    }
}
