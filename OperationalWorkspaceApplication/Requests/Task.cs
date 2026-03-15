using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{
    
    public sealed record CreateTaskRequest(
        string Title,
        string Description,  // Added
        string CreatedBy,    // Added
        DateTime DueDate);

    public sealed record AssignTaskRequest(Guid TaskId, string AssigneeId) // Missing record
    {
        public object UserId { get; internal set; }= Guid.NewGuid();
    }

    public sealed record CompleteTaskRequest(Guid TaskId);
    public sealed record GetTasksRequest(string UserId); // Added UserId for the repository call

}