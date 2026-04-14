
    using System;

    namespace OperationalWorkspaceApplication.DTOs;

    public class TicketDto
    {
        public int Id { get; set; }

        // BASIC INFO
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // STATUS & PRIORITY
        public string Status { get; set; } = "Open"; // Open, In Progress, Resolved, Closed
        public int Priority { get; set; } // 1 (Low) to 5 (Critical)

        // SAGE X3 LINK
        public string? CustomerId { get; set; } // Link this ticket to a Sage Customer

        // AUDIT
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


