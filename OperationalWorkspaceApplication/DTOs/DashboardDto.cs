using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{

    public class DashboardDto
    {
        public decimal Revenue { get; set; }
        public int OpenOrders { get; set; }
        public int InvoicesDue { get; set; }
    }

}
