using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace blog_zero_downtime_with_health_checks
{
    public class ShuttingDownHealthCheck : IHealthCheck
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<ShuttingDownHealthCheck> _logger;

        private HealthStatus _status = HealthStatus.Healthy;

        public ShuttingDownHealthCheck(
            IHostApplicationLifetime appLifetime,
            IHostEnvironment hostEnvironment,
            ILogger<ShuttingDownHealthCheck> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;

            appLifetime.ApplicationStopping.Register(() =>
            {
                _status = HealthStatus.Unhealthy;

                // We don't need to block on developer machines as there's no load balancers involved there.
                bool delayShutdown = _hostEnvironment.IsProduction();
                if (delayShutdown)
                {
                    var shutdownDelay = TimeSpan.FromSeconds(25);
                    _logger.LogInformation("Delaying shutdown for {Seconds} seconds", shutdownDelay.TotalSeconds);

                    // ASP.NET Core requests are processed on separate threads, so we can just put the main thread on sleep.
                    Thread.Sleep(shutdownDelay);

                    _logger.LogInformation("Shutdown delay completed");
                }
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
