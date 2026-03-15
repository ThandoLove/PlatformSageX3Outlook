using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.Exceptions;

public class SageAuthenticationException : Exception
{
    public SageAuthenticationException(string message) : base(message) { }
}