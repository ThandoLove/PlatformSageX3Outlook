using OperationalWorkspaceApplication.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.services

{
    // This provides the actual implementation of time for your app
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
