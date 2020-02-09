using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
using System;

namespace Core.Logging.Serilog
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IServiceProvider serviceProvider, IOptions<LogOptions> options)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

            return enrichmentConfiguration.With(new CorrelationIdEnricher(httpContextAccessor, options));
        }
    }
}
