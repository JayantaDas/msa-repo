using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Resilience
{
    public interface IPolicyProvider<T>
    {
        Policy GetCircuitBreakerPolicy();

        AsyncPolicy GetCircuitBreakerPolicyAsync();

        Policy GetWaitAndRetryPolicy();

        AsyncPolicy GetWaitAndRetryPolicyAsync();

        Policy GetWaitAndRetryCircuitBreakerPolicy();

        AsyncPolicy GetWaitAndRetryCircuitBreakerPolicyAsync();

    }
}
