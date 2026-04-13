using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    public async Task<LoginUser?> FindAccountByUsernameAsync(string username)
    {
        // For testing/mocking
        if (username == "admin")
        {
            return new LoginUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "password123", // Use hashing in real production!
                Role = "Admin"
            };
        }
        return null;
    }
}
