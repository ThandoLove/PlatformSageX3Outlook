using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{
    // 1. CREATE TASK: Used for the 'New Task' form
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        
        // Added CompanyName so it matches your TaskItem image metadata
        public string? CompanyName { get; set; }
    }

   

    public sealed record AssignTaskRequest(Guid TaskId, string AssigneeId) // Missing record
    {
        public object UserId { get; internal set; }= Guid.NewGuid();
    }

    // 3. COMPLETE TASK: Used when clicking the 'Complete' button in TaskItem
    public sealed record CompleteTaskRequest(Guid TaskId);

    // 4. DELEGATE TASK: Specifically for the Sidebar Blue Button
    public sealed record DelegateTaskRequest(Guid TaskId, string RecipientEmail);

    // 5. GET TASKS: Used by the Service to fetch the initial list
    public sealed record GetTasksRequest(string UserId);
}
