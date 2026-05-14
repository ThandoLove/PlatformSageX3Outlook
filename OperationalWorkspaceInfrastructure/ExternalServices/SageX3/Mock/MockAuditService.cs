using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;

public class MockAuditService : IAuditLogService
{
    // Retains your existing system logging signature
    public Task LogAsync(string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    // =========================================================================
    // 🟢 IMPLEMENTING YOUR EXACT IAUDITLOGSERVICE CONTRACT
    // =========================================================================

    // 🌟 Matches your contract: Returns a list of ActivityDto
    public async Task<List<ActivityDto>> GetRecentActivityForUserAsync(string userId)
    {
        return await Task.FromResult(new List<ActivityDto>
        {
            new ActivityDto(
                Guid.NewGuid(),
                "User Activity",
                $"User {userId} viewed records.",
                "Navigation",
                Guid.Empty,
                DateTime.UtcNow,
                userId,
                DateTime.UtcNow,
                "View")
        });
    }

    // 🌟 Matches your contract: Returns a list of AuditLogDto
    public async Task<List<AuditLogDto>> GetAllAuditLogsAsync()
    {
        return await Task.FromResult(new List<AuditLogDto>
        {
            new() { Action = "System Startup", User = "System", Entity = "Core", Timestamp = DateTime.UtcNow.AddDays(-1) }
        });
    }

    // 🌟 Matches your contract: Returns a list of AuditLogDto
    public async Task<List<AuditLogDto>> GetAllRecentLogsAsync()
    {
        return await Task.FromResult(new List<AuditLogDto>
        {
            new() { Action = "API Handshake", User = "SageX3Client", Entity = "Integration", Timestamp = DateTime.UtcNow.AddSeconds(-45) }
        });
    }
}
