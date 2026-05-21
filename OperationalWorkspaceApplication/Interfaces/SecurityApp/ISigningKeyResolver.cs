using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.SecurityApp

{
    /// <summary>
    /// Contract defining how cryptographic asymmetric signature verification keys are resolved from the infrastructure key vault layer.
    /// </summary>
    public interface ISigningKeyResolver
    {
        SecurityKey GetValidationKey();
        SigningCredentials GetSigningCredentials();
    }
}
