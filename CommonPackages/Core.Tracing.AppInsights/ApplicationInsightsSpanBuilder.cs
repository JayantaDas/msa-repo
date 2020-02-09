using Core.Common;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Core.Tracing.ApplicationInsights
{
    public class ApplicationInsightsSpanBuilder : ISpanBuilder
    {
        private ILogger<ApplicationInsightsSpanBuilder> logger;
        private IServiceProvider serviceProvider;

        public ISpan CurrentSpan
        {
            get
            {
                // Normally tracer itself should take care of current spans. If we were asked about current span, we just return unlinked span
                return this.CreateSpan();
            }
        }

        public ApplicationInsightsSpanBuilder(ILogger<ApplicationInsightsSpanBuilder> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public ISpan CreateSpan()
        {
            return this.CreateSpan(SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan CreateSpan(ISpanConfig config)
        {
            ApplicationInsightsSpan span = null;
            if (config.SourceLocation==SpanSourceLocation.Incoming)
            {
                span = this.serviceProvider.GetRequiredService<ISpan>() as ApplicationInsightsSpan;
                if (config.SourceType == SpanSourceType.REST)
                    span.InitForIncomingRest();
                else if (config.SourceType == SpanSourceType.Message)
                    span.InitForIncomingMessage(config.SourceName);
                else
                    span.InitForOtherIncomings();
            }

            /* Unsupported scenarios. */
            if (span == null)
            {
                logger.LogWarning($"Unsupported scenario for Create Span with Config {config}");
                span = this.serviceProvider.GetRequiredService<ISpan>() as ApplicationInsightsSpan;
            }

            return span;
        }

        public ISpan ChildSpan(ISpan parent)
        {
            return this.ChildSpan(parent, SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan ChildSpan(ISpan parentSpan, ISpanConfig config)
        {
            ApplicationInsightsSpan span = null;
            if (parentSpan is ApplicationInsightsSpan parSpan)
            {
                if (config.SourceLocation==SpanSourceLocation.Outgoing)
                {
                    if (config.SourceType == SpanSourceType.REST)
                    {
                        span.InitForOutgoingRest(parSpan);
                    }
                    else if (config.SourceType == SpanSourceType.Message)
                    {
                        span.InitForOutgoingMessage(parSpan, config.SourceName);
                    }
                }
            }

            /* Unsupported scenarios. */
            if (span == null)
            {
                logger.LogWarning($"Unsupported scenario for Child Span with Parent Span {parentSpan} and Config {config}");
                span = this.serviceProvider.GetRequiredService<ISpan>() as ApplicationInsightsSpan;
            }

            return span;
        }
    }
}
