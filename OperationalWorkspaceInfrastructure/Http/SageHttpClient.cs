using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OperationalWorkspaceInfrastructure.Http;

public class SageHttpClient : ISageHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<SageHttpClient> _logger;

    public SageHttpClient(HttpClient client, ILogger<SageHttpClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> GetAsync(
        string url,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync(
        string url,
        object payload,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
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
        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsync(
        string url,
        object payload,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string url,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return await _client.SendAsync(request, cancellationToken);
    }
}