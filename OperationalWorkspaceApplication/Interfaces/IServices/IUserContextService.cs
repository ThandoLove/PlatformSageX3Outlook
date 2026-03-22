using System;
using System.Collections.Generic;
using System.Text;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
   

    // CODE START
    public interface IUserContextService
    {
        Task<UserDto> GetCurrentUserAsync();
        Task<UserDto> GetUserAsync(string userId);
    }
    // CODE END
}
