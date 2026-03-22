using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Services
{
 
     // CODE START
    public class UserContextService : IUserContextService
    {
        public Task<UserDto> GetCurrentUserAsync()
        {
            // Replace later with real auth (JWT / SSO)
            return Task.FromResult(new UserDto
            {
                Id = "1",
                Name = "Admin User",
                Role = "Admin",
                Environment = "Production"
            });
        }

        public Task<UserDto> GetUserAsync(string userId)
        {
            return Task.FromResult(new UserDto
            {
                Id = userId,
                Name = "Employee User",
                Role = "Employee",
                Environment = "Production"
            });
        }
    }
    // CODE END
}
