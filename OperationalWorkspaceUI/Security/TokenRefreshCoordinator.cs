namespace OperationalWorkspaceUI.Security;

public class TokenRefreshCoordinator
{
    // ======================================================
    // LOCKING
    // ======================================================

    private readonly SemaphoreSlim _refreshLock =
        new(1, 1);

    // ======================================================
    // ACTIVE REFRESH TASK
    // ======================================================

    private Task<bool>? _activeRefreshTask;

    // ======================================================
    // SINGLE REFRESH PIPELINE
    // ======================================================

    public async Task<bool> ExecuteAsync(
        Func<Task<bool>> refreshAction)
    {
        await _refreshLock.WaitAsync();

        try
        {
            // ======================================================
            // EXISTING REFRESH RUNNING
            // ======================================================

            if (_activeRefreshTask != null &&
                !_activeRefreshTask.IsCompleted)
            {
                return await _activeRefreshTask;
            }

            // ======================================================
            // START NEW REFRESH
            // ======================================================

            _activeRefreshTask =
                refreshAction();

            return await _activeRefreshTask;
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}