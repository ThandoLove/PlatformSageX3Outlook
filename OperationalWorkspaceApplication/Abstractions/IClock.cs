

namespace OperationalWorkspaceApplication.IServices;

public interface IClock
{
    DateTime UtcNow { get; }
}