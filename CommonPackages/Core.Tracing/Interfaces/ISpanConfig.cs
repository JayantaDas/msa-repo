using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Core.Tracing
{
    public interface ISpanConfig
    {
        SpanSourceLocation SourceLocation { get; set; }

        string SourceName { get; set; }

        SpanSourceType SourceType { get; set; }

        /// <summary>
        /// Can be used to instruct adapter to create Span even if it may consider it existing
        /// </summary>
        bool ForceCreation { get; set; }
    }
}
