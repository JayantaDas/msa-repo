using Microsoft.Extensions.Logging;
using System;

namespace Core.Tracing
{
    public class SpanScope : ISpanScope, IDisposable
    {
        private const string ERROR_MESSAGE = "[SKIPPED] Error occurred closing span scope: ";
        private readonly ILogger<SpanScope> logger;// = LoggerFactory.GetLogger(SpanScope);

        public ISpan Span { get; set; }

        public SpanScope(ILogger<SpanScope> logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            try
            {
                this.Span.Finish();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ERROR_MESSAGE + ex.Message, ex);
            }
            try
            {
                this.Span.Tracer.RemoveSpanFromStack(this.Span);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ERROR_MESSAGE + ex.Message, ex);
            }
        }
    }
}
