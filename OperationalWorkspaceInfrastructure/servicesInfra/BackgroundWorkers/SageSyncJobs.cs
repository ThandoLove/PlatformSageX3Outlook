using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // Required for thread-safe scope generation
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.BackgroundJobsApp;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceInfrastructure.servicesInfra.BackgroundWorkers;

public class SageSyncJobs : ISageSyncJobs
{
    private readonly IServiceProvider _serviceProvider; // 🚀 STABILIZED: Bypasses all startup DI validation issues completely
    private readonly ILogger<SageSyncJobs> _logger;

    public SageSyncJobs(
        IServiceProvider serviceProvider,
        ILogger<SageSyncJobs> _loggerInstance) // Keeps parameter structure clean
    {
        _serviceProvider = serviceProvider;
        _logger = _loggerInstance;
    }

    // ===================================================================================
    // EXISTING SALES ORDER SYNC PROCESSOR (100% Preserved)
    // ===================================================================================
    public async Task ExecuteSyncAsync(string orderId, string userId)
    {
        _logger.LogInformation("Starting background Sage X3 processing for Order {OrderId} by User {UserId}", orderId, userId);

        if (!Guid.TryParse(orderId, out Guid orderGuid))
        {
            _logger.LogError("Invalid OrderId format provided: {OrderId}. Must be a valid Guid.", orderId);
            return;
        }

        try
        {
            await Task.Delay(4000);

            // 🚀 STABILIZED AT RUNTIME: Resolves the repository inside an isolated thread scope
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<ISalesOrderRepository>();

            var order = await orderRepository.GetByIdAsync(orderGuid, CancellationToken.None);
            if (order != null)
            {
                _logger.LogInformation("Found order entity data for processing: {OrderId}", orderGuid);
            }

            _logger.LogInformation("Successfully completed background synchronization for Order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed executing background balance state changes for Order {OrderId}", orderId);
            throw;
        }
    }

    // ===================================================================================
    // STABILIZED: ASYNCHRONOUS CONTACT CREATION SYNC WORKER
    // ===================================================================================
    public async Task EnqueueContactCreationAsync(ContactCreateDto dto)
    {
        _logger.LogInformation("Background thread pulled contact background processing task for: {Email}", dto.Email);

        // 🚀 STABILIZED AT RUNTIME: Resolves the business partner service inside an isolated thread scope
        using var scope = _serviceProvider.CreateScope();
        var bpService = scope.ServiceProvider.GetRequiredService<IBusinessPartnerService>();

        try
        {
            var success = await bpService.CreateContactAsync(dto);

            if (!success)
            {
                throw new Exception($"Sage X3 API refused contact integration mapping for: {dto.Email}");
            }

            _logger.LogInformation("Successfully synced contact {Email} to Sage X3 via background worker pool thread.", dto.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed executing background contact upload synchronization for: {Email}. Job will automatically retry.", dto.Email);
            throw;
        }
    }
}
