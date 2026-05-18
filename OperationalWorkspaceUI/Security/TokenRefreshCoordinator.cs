
namespace OperationalWorkspaceUI.Security;

public class TokenRefreshCoordinator
{
    private bool _refreshInProgress;

    private Task<string?>? _refreshTask;

    // ======================================================
    // SINGLE REFRESH PIPELINE
    // ======================================================

    public async Task<string?> ExecuteAsync(
        Func<Task<string?>> refreshAction)
    {
        // another refresh already running
        if (_refreshInProgress &&
            _refreshTask != null)
        {
            return await _refreshTask;
        }

        _refreshInProgress = true;

        _refreshTask = refreshAction();

        try
        {
            return await _refreshTask;
        }
        finally
        {
            _refreshInProgress = false;
            _refreshTask = null;
        }
    }
}