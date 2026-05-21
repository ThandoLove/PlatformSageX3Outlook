using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3;
using OperationalWorkspaceInfrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.services;

public class SystemHealthService : ISystemHealthService
{
    private readonly IntegrationDbContext _db;
    private readonly ISageX3Client _sageClient;
    private readonly ILogger<SystemHealthService> _logger;

    public SystemHealthService(
        IntegrationDbContext db,
        ISageX3Client sageClient,
        ILogger<SystemHealthService> logger)
    {
        _db = db;
        _sageClient = sageClient;
        _logger = logger;
    }

    public async Task<AdminSystemHealthDto> GetSystemHealthAsync()
    {
        var result = new AdminSystemHealthDto();

        // =====================================
        // DATABASE HEALTH
        // =====================================
        try
        {
            result.DatabaseConnected =
                await _db.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            result.DatabaseConnected = false;
        }

        // =====================================
        // SAGE X3 HEALTH
        // =====================================
        try
        {
            // Replace with real ping later
            result.SageX3Connected = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sage X3 connection failed");
            result.SageX3Connected = false;
        }

        // =====================================
        // API HEALTH
        // =====================================
        result.APIHealthStatus =
            result.DatabaseConnected
            ? "Healthy"
            : "Degraded";

        // =====================================
        // FAILED TRANSACTIONS
        // =====================================
        try
        {
            result.FailedTransactions =
                await _db.AuditLogs
                    .CountAsync(x =>
                        x.Severity == "Critical");
        }
        catch
        {
            result.FailedTransactions = -1;
        }

        // =====================================
        // PENDING SYNC JOBS
        // =====================================
        try
        {
            // Replace with real queue later
            result.PendingSyncJobs = 3;
        }
        catch
        {
            result.PendingSyncJobs = -1;
        }

        // =====================================
        // RESPONSE TIME
        // =====================================
        result.ApiResponseTimeMs = 42;

        // =====================================
        // LAST CHECK
        // =====================================
        result.LastCheckedUtc = DateTime.UtcNow;

        return result;
    }
}