using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspace.Domain.Enums;
using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceApplication.Extensions;

public static class TaskExtensions
{
    public static TaskStatus GetStatus(this TaskDto task)
    {
        if (task.Completed)
            return TaskStatus.Completed;

        if (task.DueDate.HasValue && task.DueDate.Value.Date < DateTime.Today)
            return TaskStatus.Pending;

        if (task.DueDate.HasValue && task.DueDate.Value.Date == DateTime.Today)
            return TaskStatus.Open;

        return TaskStatus.Assigned;
    }
}