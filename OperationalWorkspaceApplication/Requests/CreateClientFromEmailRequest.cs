using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Requests
{


    public sealed class CreateClientFromEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Subject { get; set; }
    }
}