using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public class SageX3IdentityService : ISageX3IdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SageX3IdentityService> _logger;

    public SageX3IdentityService(
        HttpClient httpClient,
        ILogger<SageX3IdentityService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SageX3UserDto?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/auth/login",
                new { login = email, password },
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<SageX3UserDto>(
                cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ValidateUserAccessAsync(
        string userId,
        string company,
        string dataset,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(company) ||
            string.IsNullOrWhiteSpace(dataset))
            return false;

        var response = await _httpClient.GetAsync(
            $"/security/validate?userId={userId}&company={company}&dataset={dataset}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadAsStringAsync(cancellationToken);

        return bool.TryParse(result, out var parsed) && parsed;
    }

    public async Task<List<string>> GetUserPermissionsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return new List<string>();

        var response = await _httpClient.GetAsync(
            $"/security/permissions/{userId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new List<string>();

        return await response.Content.ReadFromJsonAsync<List<string>>(
            cancellationToken: cancellationToken)
            ?? new List<string>();
    }
}