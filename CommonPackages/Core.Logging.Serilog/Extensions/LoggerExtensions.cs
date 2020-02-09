using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Logging.Serilog
{
    public static class LoggerExtensions
    {
        public static IApplicationBuilder UseAspNetLog(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseAspNetLog(new LogOptions()
            {
                CorrelationHeader = configuration[LoggerConstants.CONFIG_CORRELATION_HEADER] ?? LoggerConstants.DEFAULT_CORRELATION_HEADER
            }
            , configuration);
        }

        public static IApplicationBuilder UseAspNetLog(this IApplicationBuilder app, string header, IConfiguration configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseAspNetLog(new LogOptions
            {
                CorrelationHeader = header
            }, configuration);
        }

        public static IApplicationBuilder UseAspNetLog(this IApplicationBuilder app, LogOptions options, IConfiguration configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = configuration[LoggerConstants.CONFIG_CORRELATION_HEADER] ?? options.CorrelationHeader,
                UseGuidForCorrelationId = options.UseGuidForCorrelationId,
                UpdateTraceIdentifier = options.UpdateTraceIdentifier,
                IncludeInResponse = options.IncludeInResponse
            });

            return app.UseMiddleware<LoggerMiddleware>(Options.Create(options));
        }
    }
}
