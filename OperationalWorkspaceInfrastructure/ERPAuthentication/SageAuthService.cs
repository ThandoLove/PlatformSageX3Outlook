using Microsoft.Extensions.Options;
using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.Configuration;
using OperationalWorkspaceInfrastructure.Exceptions;
using OperationalWorkspaceInfrastructure.Http;
using System.Net.Http.Json;
using System.Threading;

namespace OperationalWorkspaceInfrastructure.ERPAuthentication;

public interface ISageAuthService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public class SageAuthService : ISageAuthService
{
    private readonly SageSecurityOptions _options;
    private readonly IDistributedTokenCacheService _cache;
    private readonly ISageHttpClient _httpClient;
    private const string CacheKey = "SageX3AccessToken";

    // 🚀 NEW: Semaphore pattern prevents multiple rapid Outlook clicks from flooding the Sage ERP server
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public SageAuthService(
        IOptions<SageSecurityOptions> options,
        IDistributedTokenCacheService cache,
        ISageHttpClient httpClient)
    {
        _options = options.Value;
        _cache = cache;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Double-check strategy: first check avoids locking performance hit if token is ready
        var cached = await _cache.GetAsync<string>(CacheKey);
        if (!string.IsNullOrEmpty(cached)) return cached;

        // Wait smoothly if another thread is currently fetching the token from Sage X3
        await _lock.WaitAsync(cancellationToken);
        try
        {
            // Re-verify after gaining the lock to check if the winning thread just populated it
            cached = await _cache.GetAsync<string>(CacheKey);
            if (!string.IsNullOrEmpty(cached)) return cached;

            var payload = new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                grant_type = "client_credentials"
            };

            var response = await _httpClient.PostAsync(_options.TokenEndpoint, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new SageAuthenticationException("Failed to get Sage X3 token");

            var tokenResult = await response.Content.ReadFromJsonAsync<SageTokenResponse>(cancellationToken);

            if (tokenResult == null)
                throw new SageAuthenticationException("Sage X3 returned a successful status but an empty response body.");

            // 🚀 STABILIZED EXPIRATION MATH: Protects against negative TimeSpan runtime crashes.
            // Safely subtracts a 5-minute safety buffer window. If Sage issues a short-lived token,
            // it falls back to a safe baseline lifespan rather than breaking the application.
            TimeSpan cacheLifespan;
            if (tokenResult.ExpiresIn > 300)
            {
                cacheLifespan = TimeSpan.FromSeconds(tokenResult.ExpiresIn - 300); // Expire 5 mins early
            }
            else
            {
                cacheLifespan = TimeSpan.FromSeconds(Math.Max(30, tokenResult.ExpiresIn / 2)); // Fallback baseline
            }

            await _cache.SetAsync(CacheKey, tokenResult.AccessToken, cacheLifespan);
            return tokenResult.AccessToken;
        }
        finally
        {
            _lock.Release(); // Always release your token lock
        }
    }

    private record SageTokenResponse(string AccessToken, int ExpiresIn);
}
