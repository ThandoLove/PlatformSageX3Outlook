
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace OperationalWorkspaceInfrastructure.servicesInfra.BackgroundWorkers;

public sealed class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Sage X3 Platform Background Processing Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                // Execute the asynchronous task in an isolated background scope
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred executing queued background task.");
            }
        }

        _logger.LogInformation("👋 Sage X3 Platform Background Processing Service is stopping.");
    }
}
