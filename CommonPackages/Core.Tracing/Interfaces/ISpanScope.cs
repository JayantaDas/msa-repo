using System;

namespace Core.Tracing
{
    public interface ISpanScope : IDisposable
    {
        /// <summary>The <see cref="ISpan"/> that's been scoped by this <see cref="ISpanScope"/>.</summary>
        ISpan Span { get; set; }
    }
}
