
using System;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    /// <summary>
    /// Contract defining a high-performance multi-tier lock-safe caching mechanism.
    /// </summary>
    public interface IHybridCacheService
    {
        Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> fallbackFactory, TimeSpan expirationWindow) where T : class;
    }
}
