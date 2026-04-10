using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
   

    // CODE START
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;

    }
    // CODE END
}
