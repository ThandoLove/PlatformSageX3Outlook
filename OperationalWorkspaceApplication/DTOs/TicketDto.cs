using System;

namespace OperationalWorkspaceApplication.DTOs;

public class TicketDto
{
    // BASIC INFO
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TicketNumber { get; set; } = "NEW";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // STATUS & PRIORITY

    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "3";
    

    // ASSIGNMENT (Fixes the State error)
   
    public string AssignedTo { get; set; } = string.Empty;
    // SAGE X3 LINK
    public string? CustomerName {  get; set; } = string.Empty; 

    // SAGE X3 CONTEXT FIELD
    public string SiteCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;

    // AUDIT
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

