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
        _apiKey = config["SageX3:ApiKey"] ?? string.Empty;

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {_apiKey}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    // --- Specific Implementations required by the Interface ---

    public async Task<dynamic> GetCustomersAsync()
    {
        // Calls the generic method with the 'BPCUSTOMER' entity
        // We use dynamic or a specific CustomerDto if you have one
        var result = await GetAsync<dynamic>("BPCUSTOMER", "");
        return result ?? new { };
    }

    public async Task<dynamic> GetPartnerByIdAsync(string id)
    {
        // Calls the generic method for a specific Business Partner
        var result = await GetAsync<dynamic>("BPSUPPLIER", id);
        return result ?? new { };
    }

    // --- Generic Methods ---

    public async Task<T?> GetAsync<T>(string entity, string id)
    {
        // Standard Sage REST format
        var url = string.IsNullOrEmpty(id)
            ? $"{entity}?representation={entity}.$query"
            : $"{entity}('{id}')?representation={entity}.$details";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> PostAsync<T>(string entity, T data)
    {
        var url = $"{entity}?representation={entity}.$create";
        var response = await _httpClient.PostAsJsonAsync(url, data);
        return response.IsSuccessStatusCode;
    }
}
