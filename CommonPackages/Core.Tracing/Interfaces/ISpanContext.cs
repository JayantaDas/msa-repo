using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Core.Tracing
{
    public interface ISpanContext
    {
        /// <summary>
        /// Globally unique. Every span in a trace shares this ID.
        /// </summary>
        string TraceId { get; set; }

        /// <summary>
        /// Unique within a trace. Each span within a trace contains a different ID.
        /// </summary>
        string SpanId { get; set; }

        Dictionary<string, StringValues> Headers { get; set; }
    }
}
