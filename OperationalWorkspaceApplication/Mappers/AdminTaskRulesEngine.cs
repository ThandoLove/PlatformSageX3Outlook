using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceApplication.Mappers
{
{
    public static class AdminTaskRulesEngine
    {
        // 1. SLA BREACH CALCULATION (The Red "Overdue" logic)
        public static bool IsSlaBreached(DateTime dueDate, TaskStatus status)
        {
            if (status == TaskStatus.Completed) return false;
            return DateTime.Now > dueDate;
        }

        // 2. QUEUE GROUPING (For the AdminQueuePanel)
        public static string MapToAdminQueue(TaskDto task)
        {
            if (task.Priority == TaskPriority.High && IsSlaBreached(task.DueDate, task.Status))
                return "Critical Priority";

            if (IsSlaBreached(task.DueDate, task.Status))
                return "SLA Violations";

            return "General Queue";
        }

        // 3. ASSIGNMENT LOGIC (Determines if a task can be reallocated)
        public static bool CanAdminReassign(TaskDto task)
        {
            // Admins can reassign anything that isn't already finished
            return task.Status != TaskStatus.Completed;
        }
    }
}
