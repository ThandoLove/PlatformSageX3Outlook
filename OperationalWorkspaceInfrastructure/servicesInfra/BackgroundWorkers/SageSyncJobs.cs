using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.Interfaces.BackgroundJobsApp;
using OperationalWorkspaceApplication.Interfaces.IRepository;

namespace OperationalWorkspaceInfrastructure.servicesInfra.BackgroundWorkers;

public class SageSyncJobs : ISageSyncJobs
{
    private readonly ISalesOrderRepository _orderRepository;
    private readonly ILogger<SageSyncJobs> _logger;

    public SageSyncJobs(ISalesOrderRepository orderRepository, ILogger<SageSyncJobs> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

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
            // Simulate heavy data assembly and Sage ERP web request processing
            await Task.Delay(4000);

            var order = await _orderRepository.GetByIdAsync(orderGuid, CancellationToken.None);
            if (order != null)
            {
                // Execute background synchronization adjustments safely here
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
}
