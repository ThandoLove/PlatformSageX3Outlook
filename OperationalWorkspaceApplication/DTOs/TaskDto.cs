using OperationalWorkspace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs;

public class TaskDto
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public TaskPriority Priority { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public string BpCode { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusMessage { get; set; }= string.Empty;
    public string StatusDescription { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
   
    
        

}