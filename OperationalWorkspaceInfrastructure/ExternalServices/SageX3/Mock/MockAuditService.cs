using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;

public class MockAuditService
{
    public Task LogAsync(string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}