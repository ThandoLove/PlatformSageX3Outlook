using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.DTOs;
using System;
using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceApplication.Mappers
{
    public static class AdminTaskRulesEngine
    {
        // 1. SLA BREACH CALCULATION (The Red "Overdue" logic)
        // FIX: Changed to DateTime? to match TaskDto
        public static bool IsSlaBreached(DateTime? dueDate, TaskStatus status)
        {
            if (status == TaskStatus.Completed || !dueDate.HasValue)
                return false;

            // If current time is past the due date, it's a breach
            return DateTime.Now > dueDate.Value;
        }

        // 2. QUEUE GROUPING (For the AdminQueuePanel)
        public static string MapToAdminQueue(TaskDto task)
        {
            if (task == null) return "General Queue";

            if (task.Priority == TaskPriority.High && IsSlaBreached(task.DueDate, task.Status))
                return "Critical Priority";

            if (IsSlaBreached(task.DueDate, task.Status))
                return "SLA Violations";

            return "General Queue";
        }

        // 3. ASSIGNMENT LOGIC
        public static bool CanAdminReassign(TaskDto task)
        {
            return task != null && task.Status != TaskStatus.Completed;
        }
    }
}
