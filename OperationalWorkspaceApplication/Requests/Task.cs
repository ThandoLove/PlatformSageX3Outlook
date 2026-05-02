using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{

    public class CreateTaskRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public string AssignedTo { get; set; } = "";
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
    }

    public sealed record AssignTaskRequest(Guid TaskId, string AssigneeId) // Missing record
    {
        public object UserId { get; internal set; }= Guid.NewGuid();
    }

    public sealed record CompleteTaskRequest(Guid TaskId);
    public sealed record GetTasksRequest(string UserId); // Added UserId for the repository call

}