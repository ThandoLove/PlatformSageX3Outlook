using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
   
    // CODE START
    public interface IIntegrationService
    {
        bool IsConnected();
        Task<string> GetApiHealthStatusAsync();
        Task<int> GetFailedTransactionsCountAsync();
        Task<int> GetPendingSyncJobsCountAsync();
    }
    // CODE END
}
