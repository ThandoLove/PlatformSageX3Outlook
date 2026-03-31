using System;
using OperationalWorkspace.Domain.Enums;
using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;


namespace OperationalWorkspaceUI.Models.Dashboard;

// CODE START
public class TaskItemDTO
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = TaskStatus.Pending.ToString();

    // FIX 2: Initialize Priority to prevent null reference warnings
    public string Priority { get; set; } = "Normal";
    // CODE END
}