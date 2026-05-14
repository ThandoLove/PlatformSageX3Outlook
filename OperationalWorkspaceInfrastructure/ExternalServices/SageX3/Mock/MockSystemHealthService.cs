using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;

public class MockSystemHealthService : ISystemHealthService
{
    public Task<AdminSystemHealthDto> GetSystemHealthAsync()
    {
        return Task.FromResult(new AdminSystemHealthDto
        {
            SageX3Connected = true,
            DatabaseConnected = true,
            APIHealthStatus = "Healthy",
            FailedTransactions = 2,
            PendingSyncJobs = 4,
            ApiResponseTimeMs = 31,
            LastCheckedUtc = DateTime.UtcNow
        });
    }
}