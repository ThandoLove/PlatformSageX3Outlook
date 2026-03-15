
using Polly;
using Polly.Retry;
using System.Net.Http;

namespace OperationalWorkspaceInfrastructure.Resilience;

public static class SagePolicyFactory
{
    public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        => Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}