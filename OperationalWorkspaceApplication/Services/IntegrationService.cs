using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Services
{

    // CODE START
    public class IntegrationService : IIntegrationService
    {
        public bool IsConnected()
        {
            // Later: real Sage X3 API ping
            return true;
        }

        public Task<string> GetApiHealthStatusAsync()
        {
            return Task.FromResult("Healthy");
        }

        public Task<int> GetFailedTransactionsCountAsync()
        {
            return Task.FromResult(0);
        }

        public Task<int> GetPendingSyncJobsCountAsync()
        {
            return Task.FromResult(2);
        }
    }
    // CODE END
}
