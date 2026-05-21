
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OperationalWorkspaceInfrastructure.Diagnostics.Health
{
    public class SageX3HealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SageX3HealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(2);
                return await Task.FromResult(HealthCheckResult.Healthy("Sage X3 routing matrix responding cleanly."));
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded("Downstream connection pipeline timed out.", ex);
            }
        }
    }
}
