using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.SageConfiguration;


public class SageSettings
{
    public bool UseMockData { get; set; }

    public bool AutoFallbackToMock { get; set; }

    public string BaseUrl { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string RestBaseUrl { get; set; } = string.Empty;

    public string GraphQLUrl { get; set; } = string.Empty;
}