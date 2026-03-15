using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;
public class MockSageX3Client : ISageX3Client
{
    public Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default)
        => Task.FromResult($"MOCK_CUSTOMER_{bpCode}");
}