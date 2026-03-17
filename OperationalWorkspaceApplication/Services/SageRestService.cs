
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceInfrastructure.Services;

public class SageRestService : ISageRestService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SageRestService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        // This pulls the API Key you added to your appsettings.json
        _apiKey = config["SageX3:ApiKey"] ?? string.Empty;

        // Sets up the headers Sage X3 expects
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {_apiKey}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T?> GetAsync<T>(string entity, string id)
    {
        // Standard Sage REST format: /entity('ID')?representation=entity.$details
        var url = $"{entity}('{id}')?representation={entity}.$details";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> PostAsync<T>(string entity, T data)
    {
        // Standard Sage REST format for creation: /entity?representation=entity.$create
        var url = $"{entity}?representation={entity}.$create";

        var response = await _httpClient.PostAsJsonAsync(url, data);
        return response.IsSuccessStatusCode;
    }
}
