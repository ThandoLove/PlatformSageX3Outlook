using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Responses
{
   

    public class TicketResponse
    {
        // The unique ID assigned by Sage X3 (ITNNUM)
        public string TicketNumber { get; set; } = string.Empty;

        // A short summary of the issue (ITNDES)
        public string Description { get; set; } = string.Empty;

        // The Sage Business Partner code (BPCNUM)
        public string CustomerCode { get; set; } = string.Empty;

        // The current state in Sage (e.g., "Open", "Closed", "Pending")
        public string Status { get; set; } = string.Empty;

        // The date the ticket was opened (DAT)
        public DateTime CreatedDate { get; set; }

        // How urgent the issue is in Sage (Normal, Urgent, etc.)
        public string Priority { get; set; } = "Normal";
    }

}
