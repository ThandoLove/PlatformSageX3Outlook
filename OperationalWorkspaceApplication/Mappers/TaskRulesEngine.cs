
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspace.Domain.Enums;

namespace OperationalWorkspaceApplication.Mappers
{
    public static class TaskRulesEngine
    {
        // -----------------------------
        // PRIORITY DISPLAY (UI SAFE)
        // -----------------------------
        public static string PriorityLabel(TaskPriority priority) => priority switch
        {
            TaskPriority.Urgent => "Urgent",
            TaskPriority.High => "High",
            TaskPriority.Medium => "Medium",
            TaskPriority.Low => "Low",
            _ => "Unknown"
        };

        // -----------------------------
        // PRIORITY WEIGHT (FOR LOGIC / SORTING)
        // -----------------------------
        public static int PriorityWeight(TaskPriority priority) => priority switch
        {
            TaskPriority.Urgent => 4,
            TaskPriority.High => 3,
            TaskPriority.Medium => 2,
            TaskPriority.Low => 1,
            _ => 0
        };

        // -----------------------------
        // OVERDUE CHECK (CORE RULE)
        // -----------------------------
        public static bool IsOverdue(TaskDto task)
        {
            return task.DueDate.HasValue &&
                   task.DueDate.Value.Date < DateTime.Today &&
                   !task.Completed;
        }

        // -----------------------------
        // DUE STATE (MORE INTELLIGENT THAN BOOLEAN)
        // -----------------------------
        public static TaskDueState GetDueState(TaskDto task)
        {
            if (task.Completed)
                return TaskDueState.Completed;

            if (!task.DueDate.HasValue)
                return TaskDueState.NoDate;

            var today = DateTime.Today;
            var date = task.DueDate.Value.Date;

            if (date < today)
                return TaskDueState.Overdue;

            if (date == today)
                return TaskDueState.DueToday;

            if (date <= today.AddDays(2))
                return TaskDueState.DueSoon;

            return TaskDueState.Future;
        }

        // -----------------------------
        // HUMAN READABLE LABEL
        // -----------------------------
        public static string DueLabel(TaskDto task)
        {
            if (!task.DueDate.HasValue)
                return "No due date";

            var date = task.DueDate.Value.Date;

            if (date == DateTime.Today)
                return "Today";

            if (date == DateTime.Today.AddDays(1))
                return "Tomorrow";

            if (date < DateTime.Today)
                return "Overdue";

            return task.DueDate.Value.ToString("ddd dd MMM");
        }

        // -----------------------------
        // WORKLOAD SCORING (FOR DASHBOARD)
        // -----------------------------
        public static int WorkloadScore(TaskDto task)
        {
            var priorityScore = PriorityWeight(task.Priority);

            var duePenalty = IsOverdue(task) ? 5 : 0;

            return priorityScore + duePenalty;
        }
    }

    // ---------------------------------
    // SUPPORTING ENUM (NEW LOGIC LAYER)
    // ---------------------------------
    public enum TaskDueState
    {
        NoDate,
        DueToday,
        DueSoon,
        Future,
        Overdue,
        Completed
    }
}