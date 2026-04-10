using Microsoft.EntityFrameworkCore.Storage;
using OperationalWorkspaceApplication.IServices;


namespace OperationalWorkspaceInfrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IntegrationDbContext _dbContext;

    private IDbContextTransaction? _transaction;

    public UnitOfWork(IntegrationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            return;

        _transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (_transaction != null)
                await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();

            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();

        _transaction = null;
    }
}