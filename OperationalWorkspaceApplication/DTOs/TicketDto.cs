using System;

namespace OperationalWorkspaceApplication.DTOs;

public class TicketDto
{
    public int Id { get; set; }

    // BASIC INFO
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // STATUS & PRIORITY
    public string Status { get; set; } = "Open";
    public int Priority { get; set; }

    // ASSIGNMENT (Fixes the State error)
    public string AssignedTo { get; set; } = string.Empty; // Added this line

    // SAGE X3 LINK
    public string? CustomerId { get; set; }

    // AUDIT
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
