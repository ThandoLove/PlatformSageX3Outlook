

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public interface ISageX3IdentityService
{
    Task<SageX3UserDto?> AuthenticateAsync(string email, string password);

    Task<bool> ValidateUserAccessAsync(string userId, string company, string dataset);

    Task<List<string>> GetUserPermissionsAsync(string userId);
}