using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Http;

public class SageHttpClient : ISageHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<SageHttpClient> _logger;
    private readonly IConfiguration _configuration;

    public SageHttpClient(
        HttpClient client,
        ILogger<SageHttpClient> logger,
        IConfiguration configuration)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    // ===================================================================================
    // CONFIGURATION-BASED ENVIRONMENT RESOLVER (Bypasses Layer Loops Perfectly)
    // ===================================================================================
    private string ResolveDynamicEndpointUrl(string originalUrl)
    {
        // 1. In local Development mode, fallback natively to your standard appsettings url
        var activeEnvironment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        if (activeEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            return originalUrl;
        }

        // 2. In Production, dynamically route based on the target folder specified in your appsettings.production.json mapping keys
        // To maintain 100% boundary safety, we read the default config endpoint directly from the platform layout configuration profile matrix
        string targetEnv = _configuration["SageX3:Endpoint"] ?? "PROD";
        string? mappedBaseUrl = _configuration[$"SageEnvironmentsMapping:{targetEnv}:BaseUrl"];

        if (string.IsNullOrWhiteSpace(mappedBaseUrl))
        {
            return originalUrl;
        }

        // 3. Swap out your generic placeholder domain for your active, live server instance URL
        string defaultServerDomain = _configuration["SageX3:BaseUrl"] ?? "https://your-sage-server";

        if (originalUrl.StartsWith(defaultServerDomain, StringComparison.OrdinalIgnoreCase))
        {
            return originalUrl.Replace(defaultServerDomain, mappedBaseUrl, StringComparison.OrdinalIgnoreCase);
        }

        return originalUrl;
    }

    // ======================================================
    // DATA TRANSMISSION PIPELINES (Re-wired to use Resolved Endpoints)
    // ======================================================
    public async Task<HttpResponseMessage> GetAsync(
        string url,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        string dynamicUrl = ResolveDynamicEndpointUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Get, dynamicUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync(
        string url,
        object payload,
        CancellationToken cancellationToken = default)
    {
        string dynamicUrl = ResolveDynamicEndpointUrl(url);
        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, dynamicUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync(
        string url,
        object payload,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        string dynamicUrl = ResolveDynamicEndpointUrl(url);
        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, dynamicUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsync(
        string url,
        object payload,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        string dynamicUrl = ResolveDynamicEndpointUrl(url);
        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Put, dynamicUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string url,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        string dynamicUrl = ResolveDynamicEndpointUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Delete, dynamicUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }
}

