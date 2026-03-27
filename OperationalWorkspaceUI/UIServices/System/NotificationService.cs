using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System
{
    public class NotificationService
    {
        public event Func<string, string, Task>? OnNotify;

        // CORE METHOD
        public Task Show(string message, string type = "info")
        {
            if (OnNotify != null)
                return OnNotify.Invoke(message, type);

            return Task.CompletedTask;
        }

        // ✅ CLEAN API (what your UI expects)
        public Task Success(string message) => Show(message, "success");

        public Task Error(string message) => Show(message, "error");

        public Task Warning(string message) => Show(message, "warning");

        public Task Info(string message) => Show(message, "info");


    }
}