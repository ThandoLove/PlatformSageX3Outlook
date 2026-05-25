
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
