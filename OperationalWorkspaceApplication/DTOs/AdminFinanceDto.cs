using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
    // CODE START
    public class AdminFinanceDto
    {
        public decimal OutstandingReceivables { get; set; }
        public int HighRiskAccounts { get; set; }

        public decimal TotalOutstandingReceivables { get; set; }
        public decimal TotalSalesThisMonth { get; set; }

    }
    // CODE END
}
