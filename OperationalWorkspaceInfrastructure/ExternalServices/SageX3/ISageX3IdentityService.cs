

using System.Collections.Generic;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public interface ISageX3IdentityService
{
    // ======================================================
    // AUTHENTICATE USER AGAINST SAGE X3
    // ======================================================

    Task<SageX3UserDto?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    // ======================================================
    // VALIDATE TENANT ACCESS
    // ======================================================

    Task<bool> ValidateUserAccessAsync(
        string userId,
        string company,
        string dataset,
        CancellationToken cancellationToken = default);

    // ======================================================
    // LOAD USER PERMISSIONS
    // ======================================================

    Task<List<string>> GetUserPermissionsAsync(
        string userId,
        CancellationToken cancellationToken = default);
}