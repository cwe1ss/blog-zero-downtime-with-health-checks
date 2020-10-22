using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace blog_zero_downtime_with_health_checks
{
    public class ShuttingDownHealthCheck : IHealthCheck
    {
        private HealthStatus _status = HealthStatus.Healthy;

        public ShuttingDownHealthCheck(IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("Shutting down");
                _status = HealthStatus.Unhealthy;
            });
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var result = new HealthCheckResult(_status);
            return Task.FromResult(result);
        }
    }
}
