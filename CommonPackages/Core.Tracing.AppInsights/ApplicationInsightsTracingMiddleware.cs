using Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Core.Tracing.ApplicationInsights
{
    public class ApplicationInsightsTracingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ApplicationInsightsTracingMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightsTracingMiddleware"/> class.
        /// Creates a new instance of the Application Insights Tracing Middleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public ApplicationInsightsTracingMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<ApplicationInsightsTracingMiddleware> logger)
        {
            this.next = next;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITracer tracer, ISpanConfig spanConfig, ISpanContext traceContext)
        {
            if (context.Request.Headers.ContainsKey(SpanConstants.HEADER_REQUESTID))
                traceContext.Headers.Add(SpanConstants.HEADER_REQUESTID, context.Request.Headers[SpanConstants.HEADER_REQUESTID]);
            if (context.Request.Headers.ContainsKey(SpanConstants.HEADER_TRACEPARENT))
                traceContext.Headers.Add(SpanConstants.HEADER_TRACEPARENT, context.Request.Headers[SpanConstants.HEADER_TRACEPARENT]);
            if (context.Request.Headers.ContainsKey(SpanConstants.HEADER_AZURE_TRACEPARENT))
                traceContext.Headers.Add(SpanConstants.HEADER_AZURE_TRACEPARENT, context.Request.Headers[SpanConstants.HEADER_AZURE_TRACEPARENT]);
            if (context.Request.Headers.ContainsKey(SpanConstants.HEADER_TRACE_ID))
                traceContext.Headers.Add(SpanConstants.HEADER_TRACE_ID, context.Request.Headers[SpanConstants.HEADER_TRACE_ID]);

            spanConfig.ForceCreation = true;
            spanConfig.SourceLocation = SpanSourceLocation.Incoming;
            spanConfig.SourceName = "REST";
            spanConfig.SourceType = SpanSourceType.REST;
            var span = tracer.CreateSpan(spanConfig);
            span.AddTag(SpanConstants.TAG_REQUEST_URL, context.Request.Host + context.Request.Path);
            span.AddTag(SpanConstants.TAG_REQUEST_METHOD, context.Request.Method);

            span.Start();

            await this.next(context);

            span.Finish();
        }
    }
}
