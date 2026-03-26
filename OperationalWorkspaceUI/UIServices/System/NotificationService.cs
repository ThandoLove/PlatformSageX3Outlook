
   using System;
   using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.System
{
    public class NotificationService
        {
            public event Func<string, string, Task> OnNotify;

            public Task ShowNotification(string message, string type = "info")
            {
                if (OnNotify != null)
                    return OnNotify.Invoke(message, type);

                return Task.CompletedTask;
            }
        }
    }