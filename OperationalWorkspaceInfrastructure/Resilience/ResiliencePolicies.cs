using System;
using System.Net;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace OperationalWorkspaceInfrastructure.Resilience;

public static class ResiliencePolicies
{
    // The modern equivalent of IAsyncPolicy is ResiliencePipeline<HttpResponseMessage>
    public static ResiliencePipeline<HttpResponseMessage> GetResiliencePolicy()
    {
        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            // 1. TIMEOUT STRATEGY (Outer Layer)
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(10)
            })
            // 2. RETRY STRATEGY
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromSeconds(2),
                // Handle transient HTTP errors (5xx, 408) plus 429 Too Many Requests
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500 ||
                                       r.StatusCode == HttpStatusCode.RequestTimeout ||
                                       r.StatusCode == HttpStatusCode.TooManyRequests)
            })
            // 3. CIRCUIT BREAKER STRATEGY (Inner Layer)
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = 0.5, // Break if 50% of requests fail
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 5, // Equivalent to allowed events before break
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500 ||
                                       r.StatusCode == HttpStatusCode.RequestTimeout)
            })
            .Build();
    }
}
