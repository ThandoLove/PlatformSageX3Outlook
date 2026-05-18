
namespace OperationalWorkspaceAPI.Authentication;

public interface IAuthProvider
{
    Task<AuthResult> AuthenticateAsync(
        string username,
        string password);
}