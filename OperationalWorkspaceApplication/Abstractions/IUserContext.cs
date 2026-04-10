
namespace OperationalWorkspaceApplication.IServices;

public interface IUserContext
{
    string UserId { get; }
    string? Email { get; }
    string UserName { get; }
}