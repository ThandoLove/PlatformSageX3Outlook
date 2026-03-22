using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    
    public interface IAuditLogService
    {
        Task<List<ActivityDto>> GetRecentActivityForUserAsync(string userId);
        Task<List<AuditLogDto>> GetAllAuditLogsAsync();
        Task<List<AuditLogDto>> GetAllRecentLogsAsync();
    }
    // CODE END
}
