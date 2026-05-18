using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;


public class MockSageX3IdentityService : ISageX3IdentityService
{
    public async Task<SageX3UserDto?>
        AuthenticateAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await Task.FromResult(
            new SageX3UserDto
            {
                UserId = Guid.NewGuid().ToString(),

                Username = email,

                Email = email,

                Company = "DEMO",

                Dataset = "MAIN",

                Role = "Administrator",

                IsActive = true,

                Permissions = new List<string>
                {
                    "CONTACT_VIEW",
                    "CONTACT_CREATE",
                    "CONTACT_EDIT",
                    "OPPORTUNITY_VIEW",
                    "OPPORTUNITY_CREATE"
                }
            });
    }

    public async Task<bool>
        ValidateUserAccessAsync(
            string userId,
            string company,
            string dataset,
            CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(true);
    }

    public async Task<List<string>>
        GetUserPermissionsAsync(
            string userId,
            CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(
            new List<string>
            {
                "CONTACT_VIEW",
                "CONTACT_CREATE",
                "CONTACT_EDIT",
                "OPPORTUNITY_VIEW",
                "OPPORTUNITY_CREATE"
            });
    }
}