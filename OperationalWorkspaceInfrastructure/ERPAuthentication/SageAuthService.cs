using OperationalWorkspaceInfrastructure.Caching;
using OperationalWorkspaceInfrastructure.Configuration;
using OperationalWorkspaceInfrastructure.Http;
using OperationalWorkspaceInfrastructure.Exceptions;
using System.Net.Http.Json;


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

    public SageAuthService(SageSecurityOptions options,
                            IDistributedTokenCacheService cache,
                            ISageHttpClient httpClient)
    {
        _options = options;
        _cache = cache;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync<string>(CacheKey);
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

        // Add this check to satisfy the compiler and handle empty bodies
        if (tokenResult == null)
            throw new SageAuthenticationException("Sage X3 returned a successful status but an empty response body.");

        await _cache.SetAsync(CacheKey, tokenResult.AccessToken, TimeSpan.FromMinutes(tokenResult.ExpiresIn - 60));
        return tokenResult.AccessToken;

    }

    private record SageTokenResponse(string AccessToken, int ExpiresIn);
}