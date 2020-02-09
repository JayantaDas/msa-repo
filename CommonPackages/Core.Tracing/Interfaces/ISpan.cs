using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Core.Tracing
{
    public interface ISpan
    {
        /// <summary>
        /// Get reference to the implementation of <seealso cref="ISpanBuilder"/> this span adapter works with.
        /// Reference to the relevant Span Builder
        /// </summary>
        ISpanBuilder SpanBuilder { get; }

        /// <summary>
        /// Get reference to the implementation of <seealso cref="ITracer"/> this span adapter works with.
        /// Reference to the relevant Span Builder
        /// </summary>
        ITracer Tracer { get; }

        /// <summary>
        /// Adds Span Adapter to the list of Adapters
        /// </summary>
        /// <param name="span">Span Adapter to be added</param>
        void AddAdapter(ISpan span);

        /// <summary>
        /// Explicitly creates and returns child span of this span
        /// </summary>
        /// <returns>Span representing created child</returns>
        ISpan ChildSpan();

        /// <summary>
        /// Changes the name of the current span
        /// </summary>
        /// <param name="name">Name to be set</param>
        /// <returns>The current span</returns>
        ISpan SetName(string name);

        /// <summary>
        /// Adds tag to the current span
        /// </summary>
        /// <param name="key">Tag key to be added</param>
        /// <param name="value">Tag value to be added</param>
        /// <returns>The current span</returns>
        ISpan AddTag(string key, string value);

        /// <summary>
        /// Start the current span
        /// </summary>
        /// <returns>The current span</returns>
        ISpan Start();

        /// <summary>
        /// Finish the current span
        /// </summary>
        void Finish();

        /// <summary>
        /// Add error info to the current span
        /// </summary>
        /// <param name="ex">Exception to add</param>
        ISpan AddError(Exception ex);

        /// <summary>
        /// Make this span as a current Span
        /// </summary>
        /// <returns>The current span</returns>
        ISpan SetInScope();

        /// <summary>
        /// Adds headers with propagation info for tracing into provided headers Map
        /// </summary>
        void AddPropagationHeaders();
    }
}
