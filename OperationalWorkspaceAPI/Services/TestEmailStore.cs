using OperationalWorkspaceAPI.Models;

namespace OperationalWorkspaceAPI.Services;

public static class TestEmailStore
{
    private static TestEmailDto? _latest;

    public static TestEmailDto? Latest
    {
        get => _latest;
        set => _latest = value;
    }
}
