using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceInfrastructure.Services;

public class SageRestService : ISageRestService
{
    private readonly HttpClient _httpClient;
    private readonly IUserContextService _userContext;
    private readonly string _defaultApiKey;

    public SageRestService(HttpClient httpClient, IConfiguration config, IUserContextService userContext)
    {
        _httpClient = httpClient;
        _userContext = userContext;
        _defaultApiKey = config["SageX3:ApiKey"] ?? string.Empty;

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T?> GetAsync<T>(string entity, string id)
    {
        var user = await _userContext.GetCurrentUserAsync();

        // Use the environment from the user claim as the Sage Folder/Tenant
        string folder = user.Environment;

        // Construct URL: e.g., https://sage.com...
        var url = string.IsNullOrEmpty(id)
            ? $"{folder}/{entity}?representation={entity}.$query"
            : $"{folder}/{entity}('{id}')?representation={entity}.$details";

        // Apply Authorization (Using default key, or user-specific logic if needed)
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _defaultApiKey);

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<dynamic> GetCustomersAsync()
    {
        var result = await GetAsync<dynamic>("BPCUSTOMER", "");
        return result ?? new { };
    }

    public async Task<dynamic> GetPartnerByIdAsync(string id)
    {
        var result = await GetAsync<dynamic>("BPSUPPLIER", id);
        return result ?? new { };
    }

    public async Task<bool> PostAsync<T>(string entity, T data)
    {
        var user = await _userContext.GetCurrentUserAsync();
        var url = $"{user.Environment}/{entity}?representation={entity}.$create";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _defaultApiKey);
        var response = await _httpClient.PostAsJsonAsync(url, data);
        return response.IsSuccessStatusCode;
    }
}
