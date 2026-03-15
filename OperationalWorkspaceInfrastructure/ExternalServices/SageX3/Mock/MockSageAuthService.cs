using OperationalWorkspaceInfrastructure.ERPAuthentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;


public class MockSageAuthService : ISageAuthService
{
    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        => Task.FromResult("MOCK_SAGE_TOKEN");
}