
using OperationalWorkspaceInfrastructure.ERPAuthentication;
using OperationalWorkspaceInfrastructure.Exceptions;
using OperationalWorkspaceInfrastructure.Http;



namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public interface ISageX3Client
{
    Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default);
}

public class SageX3Client : ISageX3Client
{
    private readonly ISageAuthService _authService;
    private readonly ISageHttpClient _httpClient;
    private readonly string _baseUrl;

    public SageX3Client(ISageAuthService authService, ISageHttpClient httpClient, string baseUrl)
    {
        _authService = authService;
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<string> GetCustomerDataAsync(string bpCode, CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetAccessTokenAsync(cancellationToken);
        var response = await _httpClient.GetAsync($"{_baseUrl}/customers/{bpCode}", token, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new SageIntegrationException("Failed to retrieve customer data");

        return await response.Content.ReadAsStringAsync(cancellationToken);

    }
}