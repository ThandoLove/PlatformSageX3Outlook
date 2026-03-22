using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
public class AdminErpDto
    {
        public int TotalOrdersToday { get; set; }
        public int InvoicesGenerated { get; set; }
        public int OverdueInvoices { get; set; }
        public int StockAlerts { get; set; }
    }
    // CODE END
}
