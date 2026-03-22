using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{

    public class ApprovalDto
    {
        public string Type { get; set; } = string.Empty; // e.g., "Invoice Approval", "Leave Request"
        public string Entity { get; set; } = string.Empty; // e.g., "Invoice #12345", "Leave Request for John Doe"
        public string Status { get; set; } = string.Empty;
    }
}
