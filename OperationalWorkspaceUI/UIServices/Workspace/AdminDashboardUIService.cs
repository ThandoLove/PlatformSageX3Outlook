using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.Workspace;

public class AdminDashboardUIService
{
    public async Task<List<ActivityDto>> GetRecentActivityAsync()
    {
        await Task.Delay(100);
        return new List<ActivityDto>();
    }

    public async Task<int> GetTotalUsersAsync()
    {
        await Task.Delay(100);
        return 1284;
    }

    // 🟢 FIX: Added missing method required by your dashboard layout template
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        await Task.Delay(50);
        return new UserDto
        {
            Name = "System Administrator",
            Role = "Global Administrator"
        };
    }

    // 🟢 FIX: Added missing log lookup matching your Blazor UI list iteration structures
    public async Task<List<AuditLogDto>> GetRecentLogsAsync()
    {
        await Task.Delay(50);
        return new List<AuditLogDto>
        {
            new() { Action = "User Login", User = "admin@workspace.com", Timestamp = DateTime.Now.AddMinutes(-5) },
            new() { Action = "ERP Config Update", User = "manager@workspace.com", Timestamp = DateTime.Now.AddHours(-2) }
        };
    }
}

// 🟢 FIX: Temporary fallback DTO shapes matching UI reference assignments
