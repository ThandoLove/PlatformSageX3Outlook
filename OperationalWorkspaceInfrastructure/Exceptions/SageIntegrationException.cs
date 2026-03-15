using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.Exceptions;

public class SageIntegrationException : Exception
{
    public SageIntegrationException(string message) : base(message) { }
}