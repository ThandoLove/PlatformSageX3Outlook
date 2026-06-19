using OperationalWorkspaceApplication.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;


namespace OperationalWorkspaceInfrastructure.servicesInfra

{
    // This provides the actual implementation of time for your app
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
