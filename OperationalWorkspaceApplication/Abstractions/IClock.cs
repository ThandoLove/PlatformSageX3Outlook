

namespace OperationalWorkspaceApplication.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}