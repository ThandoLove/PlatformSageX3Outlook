using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{
    public class TicketRequest
    {
        public string BPCNUM_0 { get; set; } = string.Empty;  // Customer ID in Sage
        public string ITNDES_0 { get; set; } = string.Empty;// Description
        public DateTime DAT_0 { get; set; } // Date
        public string PRI_0 { get; set; } = string.Empty;   // Priority
    }

}
