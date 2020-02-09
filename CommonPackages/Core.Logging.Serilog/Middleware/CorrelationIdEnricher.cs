using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Core.Logging.Serilog
{
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private IHttpContextAccessor httpContextAccessor;
        private readonly LogOptions options;

        public CorrelationIdEnricher(IHttpContextAccessor httpContextAccessor, IOptions<LogOptions> options)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(
            propertyFactory.CreateProperty(options.CorrelationHeader, this.GetCorrelationId()));

            logEvent.AddOrUpdateProperty(
            propertyFactory.CreateProperty(options.ParentId, this.GetParentId()));
        }

        private string GetParentId()
        {
            var isParentId = this.httpContextAccessor?.HttpContext?.Request?.Headers.ContainsKey("ParentId");

            if (isParentId != null && isParentId == true)
            {
                return this.httpContextAccessor.HttpContext.Request?.Headers["ParentId"];
            }

            return Guid.NewGuid().ToString();
        }

        private string GetCorrelationId()
        {
            if (this.httpContextAccessor.HttpContext != null)
            {
                return this.httpContextAccessor.HttpContext.TraceIdentifier;
            }
            else
            {
                return "-1000";
            }
        }
    }
}
