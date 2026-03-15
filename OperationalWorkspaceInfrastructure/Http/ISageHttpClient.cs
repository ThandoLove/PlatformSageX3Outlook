using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceInfrastructure.Http;


public interface ISageHttpClient
{
    Task<HttpResponseMessage> GetAsync(
        string url,
        string accessToken,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> PostAsync(
        string url,
        object payload,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> PostAsync(
        string url,
        object payload,
        string accessToken,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> PutAsync(
        string url,
        object payload,
        string accessToken,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(
        string url,
        string accessToken,
        CancellationToken cancellationToken = default);
}