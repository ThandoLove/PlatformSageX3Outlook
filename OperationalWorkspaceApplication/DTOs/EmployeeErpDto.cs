using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
    // CODE START
    public class EmployeeErpDto
    {
        public int MyOpenOrders { get; set; }
        public int InvoicesDue { get; set; }
        public int PendingDeliveries { get; set; }
        public int StockAlerts { get; set; }
    }
    // CODE END
}
