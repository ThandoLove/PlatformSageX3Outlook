using OperationalWorkspace.Domain.Enums;
using System;

namespace OperationalWorkspaceApplication.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public TaskPriority Priority { get; set; }

        public string AssignedTo { get; set; } = string.Empty;

        public string? CompanyName { get; set; }

        public bool Completed { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }


        public string? OutlookItemId { get; set; }

        public string? SageEntityId { get; set; }

        public string? StatusDescription { get; set; }

        public string? Description { get; set; }

        public string SourceEmailSubject { get; set; } = "";
        public string SourceSender { get; set; } = "";
        public string Category { get; set; } = ""; // Sales, Support, Finance
    }
}