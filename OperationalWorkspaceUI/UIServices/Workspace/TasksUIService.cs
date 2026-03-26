
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace OperationalWorkspaceUI.UIServices.Workspace

{
    public class TasksUIService
        {
            private readonly List<string> _tasks = new();

            public Task<List<string>> GetTasksAsync()
            {
                return Task.FromResult(_tasks);
            }

            public Task AddTaskAsync(string task)
            {
                _tasks.Add(task);
                return Task.CompletedTask;
            }
        }
    }