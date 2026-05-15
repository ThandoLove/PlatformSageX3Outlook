using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;


namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public class SageX3IdentityService : ISageX3IdentityService
{
    private readonly HttpClient _httpClient;

    public SageX3IdentityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SageX3UserDto?> AuthenticateAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/login", new
        {
            login = email,
            password = password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<SageX3UserDto>();
    }

    public async Task<bool> ValidateUserAccessAsync(string userId, string company, string dataset)
    {
        var response = await _httpClient.GetAsync(
            $"/security/validate?userId={userId}&company={company}&dataset={dataset}"
        );

        if (!response.IsSuccessStatusCode)
            return false;

        return bool.Parse(await response.Content.ReadAsStringAsync());
    }

    public async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"/security/permissions/{userId}");

        if (!response.IsSuccessStatusCode)
            return new List<string>();

        return await response.Content.ReadFromJsonAsync<List<string>>() ?? new();
    }
}