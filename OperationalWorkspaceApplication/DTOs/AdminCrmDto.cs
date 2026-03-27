using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
   
public class AdminCrmDto
    {
        public int ActiveCustomers { get; set; }
        public int NewLeadsToday { get; set; }
        public int OpenOpportunities { get; set; }
        public List<ClientDTO>? TopClients { get; set; }
    }
    // CODE END
}
