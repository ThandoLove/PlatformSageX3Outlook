using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceInfrastructure.Caching
{
    public class HybridCacheService : IHybridCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<HybridCacheService> _logger;
        private static readonly SemaphoreSlim CacheExecutionLock = new SemaphoreSlim(1, 1);

        public HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache, ILogger<HybridCacheService> logger)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> fallbackFactory, TimeSpan expirationWindow) where T : class
        {
            // Tier 1: Check ultra-fast local in-memory pool cache layer (L1)
            if (_memoryCache.TryGetValue(cacheKey, out T? localizedResult))
            {
                return localizedResult;
            }

            // Sync using single-active-refresh locking mechanisms to eliminate cache stampedes under network spikes
            await CacheExecutionLock.WaitAsync();
            try
            {
                // Re-evaluate localized state after lock clearance
                if (_memoryCache.TryGetValue(cacheKey, out localizedResult))
                {
                    return localizedResult;
                }

                // Tier 2: Check distributed Redis cache cluster layer (L2)
                var rawCachedJsonString = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(rawCachedJsonString))
                {
                    var hydrationResult = JsonSerializer.Deserialize<T>(rawCachedJsonString);
                    if (hydrationResult != null)
                    {
                        // Sync local cache tier back up instantly
                        _memoryCache.Set(cacheKey, hydrationResult, expirationWindow.Divide(2));
                        return hydrationResult;
                    }
                }

                // Tier 3: Critical Data Cache Miss. Route processing down to the downstream infrastructure providers
                _logger.LogInformation("Operational cache miss encountered for lookup index key: '{CacheKey}'. Calling downstream backend data factory...", cacheKey);
                var resolvedBusinessPayload = await fallbackFactory();

                if (resolvedBusinessPayload != null)
                {
                    // Cache the fresh dataset across both localized memory pools and distributed cluster instances concurrently
                    _memoryCache.Set(cacheKey, resolvedBusinessPayload, expirationWindow.Divide(2));

                    var executionCachingParameters = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expirationWindow
                    };

                    var serializedDataString = JsonSerializer.Serialize(resolvedBusinessPayload);
                    await _distributedCache.SetStringAsync(cacheKey, serializedDataString, executionCachingParameters);
                }

                return resolvedBusinessPayload;
            }
            finally
            {
                CacheExecutionLock.Release();
            }
        }
    }
}
