using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IAccountRepository
{
    Task<UserAccount?> FindAccountByUsernameAsync(string username);
    Task<UserAccount?> FindAccountByIdAsync(string id);

    Task UpdateAsync(UserAccount user);

    Task SaveRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task UpdateRefreshTokenAsync(RefreshToken stored);
}