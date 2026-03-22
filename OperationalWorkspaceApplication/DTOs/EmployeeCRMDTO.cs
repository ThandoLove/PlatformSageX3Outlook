using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
  
    public class EmployeeCRMDTO
    {
        public string TopCustomer { get; set; } = string.Empty;
        public decimal TopCustomerValue { get; set; }
        public string RecentInteraction { get; set; } = string.Empty;
        public int OpenOpportunities { get; set; }
    }
  
}
