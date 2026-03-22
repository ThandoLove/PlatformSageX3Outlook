using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
   

    public interface IDashboardService
    {
        Task<EmployeeDashboardDTO> GetEmployeeDashboardAsync(string userId);
        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }

}
