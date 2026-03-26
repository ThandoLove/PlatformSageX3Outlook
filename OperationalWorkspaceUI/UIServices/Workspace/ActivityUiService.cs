
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class ActivityUIService
        {
            private readonly List<string> _activities = new();

            public Task<List<string>> GetActivitiesAsync()
            {
                return Task.FromResult(_activities);
            }

            public Task LogActivityAsync(string message)
            {
                _activities.Add($"{DateTime.Now}: {message}");
                return Task.CompletedTask;
            }
        }
    }