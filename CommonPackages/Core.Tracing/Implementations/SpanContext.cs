using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Tracing
{
    public class SpanContext : ISpanContext
    {
        public string TraceId { get; set; }

        public string SpanId { get; set; }

        public Dictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();

        public SpanContext()
            : this(null, null)
        {
        }

        public SpanContext(string traceId, string spanId)
        {
            this.TraceId = traceId ?? ActivityTraceId.CreateRandom().ToHexString();
            this.SpanId = spanId ?? ActivitySpanId.CreateRandom().ToHexString();
        }

        public void Init(ISpanContext parent)
        {
            this.TraceId = parent.TraceId;
            this.SpanId = ActivitySpanId.CreateRandom().ToHexString();
        }
    }
}
