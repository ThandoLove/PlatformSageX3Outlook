namespace OperationalWorkspaceAPI.SecurityAPI;

public interface ISecurityContext
{
    string UserId { get; }
    string Email { get; }
    string Role { get; }

    string Company { get; }
    string Dataset { get; }

    bool IsAuthenticated { get; }
}