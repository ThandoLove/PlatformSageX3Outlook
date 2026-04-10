
namespace OperationalWorkspaceApplication.IServices;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}