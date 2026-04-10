using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceAPI.Services;

public class MockSageRestService : ISageRestService
{
    public Task<T?> GetAsync<T>(string entity, string id)
    {
        // Return default or simple mock object depending on T
        return Task.FromResult<T?>(default);
    }

    public Task<bool> PostAsync<T>(string entity, T data)
    {
        // Pretend the operation always succeeds in dev
        return Task.FromResult(true);
    }
}
