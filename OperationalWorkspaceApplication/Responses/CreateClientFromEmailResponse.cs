using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Responses
{
  
    public sealed class CreateClientFromEmailResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
