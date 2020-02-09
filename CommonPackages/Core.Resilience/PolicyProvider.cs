using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Wrap;

namespace Core.Resilience
{
    public class PolicyProvider<T> : IPolicyProvider<T>
    {
        private readonly ILogger<PolicyProvider<T>> _logger;
        private readonly IConfiguration _configuration;
        private int retries = 0;
        private bool exponentialBackoff = false;
        private int backoffSeconds = 2;

        public PolicyProvider(ILogger<PolicyProvider<T>> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            this.retries = Convert.ToInt32(configuration["retries"]);
            this.exponentialBackoff = Convert.ToBoolean(configuration["exponentialBackoff"]);
            this.backoffSeconds = Convert.ToInt32(configuration["backoffSeconds"]);
        }
        public Policy GetCircuitBreakerPolicy()
        {
            // Define our CircuitBreaker policy: Break if the action fails 4 times in a row.
            CircuitBreakerPolicy circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 4,
                    durationOfBreak: TimeSpan.FromSeconds(3),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.LogDebug(".Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!..due to: " + ex.Message);
                    },
                    onReset: () => _logger.LogDebug(".Breaker logging: Call ok! Closed the circuit again!"),
                    onHalfOpen: () => _logger.LogDebug(".Breaker logging: Half-open: Next call is a trial!"));

            return circuitBreakerPolicy;
        }


        public AsyncPolicy GetCircuitBreakerPolicyAsync()
        {
            // Define our CircuitBreaker policy: Break if the action fails 4 times in a row.
            AsyncCircuitBreakerPolicy circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 4,
                    durationOfBreak: TimeSpan.FromSeconds(10),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.LogDebug(".Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!..due to: " + ex.Message);
                    },
                    onReset: () => _logger.LogDebug(".Breaker logging: Call ok! Closed the circuit again!"),
                    onHalfOpen: () => _logger.LogDebug(".Breaker logging: Half-open: Next call is a trial!"));

            return circuitBreakerPolicy;
        }

        public Policy GetWaitAndRetryCircuitBreakerPolicy()
        {
            Policy waitAndRetryPolicy = this.GetWaitAndRetryPolicy();

            // Define our CircuitBreaker policy: Break if the action fails 4 times in a row.
            Policy circuitBreakerPolicy = this.GetCircuitBreakerPolicy();

            PolicyWrap resilienceStrategy = Policy.Wrap(waitAndRetryPolicy, circuitBreakerPolicy);

            return resilienceStrategy;
        }

        public AsyncPolicy GetWaitAndRetryCircuitBreakerPolicyAsync()
        {
            AsyncPolicy waitAndRetryPolicy = this.GetWaitAndRetryPolicyAsync();

            // Define our CircuitBreaker policy: Break if the action fails 4 times in a row.
            AsyncPolicy circuitBreakerPolicy = this.GetCircuitBreakerPolicyAsync();

            // As demo07: we combine the waitAndRetryPolicy and circuitBreakerPolicy into a PolicyWrap, using the *static* Policy.Wrap syntax.
            AsyncPolicyWrap resilienceStrategy = Policy.WrapAsync(waitAndRetryPolicy, circuitBreakerPolicy);

            return resilienceStrategy;
        }

        public Policy GetWaitAndRetryPolicy()
        {
            RetryPolicy waitAndRetryPolicy = Policy
                .Handle<Exception>(e => !(e is BrokenCircuitException)) // Exception filtering!  We don't retry if the inner circuit-breaker judges the underlying system is out of commission!
                .WaitAndRetryForever(
                    attempt => TimeSpan.FromMilliseconds(200),
                    (exception, calculatedWaitDuration) =>
                    {
                        _logger.LogDebug(".Log,then retry: " + exception.Message);
                        this.retries++;
                    });

            return waitAndRetryPolicy;
        }

        public AsyncPolicy GetWaitAndRetryPolicyAsync()
        {
            AsyncRetryPolicy waitAndRetryPolicy = Policy
                .Handle<Exception>(e => !(e is BrokenCircuitException)) // Exception filtering!  We don't retry if the inner circuit-breaker judges the underlying system is out of commission!
                .WaitAndRetryForeverAsync(
                    attempt => TimeSpan.FromMilliseconds(200),
                    (exception, calculatedWaitDuration) =>
                    {
                        _logger.LogDebug(".Log,then retry: " + exception.Message);
                        this.retries++;
                    });

            return waitAndRetryPolicy;
        }
    }
}
