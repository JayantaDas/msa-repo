using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Core.Tracing
{
    public class SpanConfig : ISpanConfig
    {
        public static readonly SpanConfig None = new SpanConfig();

        public SpanSourceLocation SourceLocation { get; set; } = SpanSourceLocation.Internal;

        public string SourceName { get; set; }

        public SpanSourceType SourceType { get; set; } = SpanSourceType.Unknown;

        /// <summary>
        /// Can be used to instruct adapter to create Span even if it may consider it existing
        /// </summary>
        public bool ForceCreation { get; set; } = false;
    }
}
