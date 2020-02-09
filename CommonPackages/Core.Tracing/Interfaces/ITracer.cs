namespace Core.Tracing
{
    public interface ITracer
    {
        ISpanBuilder SpanBuilder { get; set; }

        bool IsStackTraceOnError { get; set; }

        /// <summary>
        /// Adds Span Adapter to the list of Adapters
        /// </summary>
        /// <param name="spanBuilder">Span Builder to be added</param>
        void AddAdapter(ISpanBuilder spanBuilder);

        /// <summary>
        /// Create a next span for the current Handler's context. If context is not available we raise an 
        /// exception. If tracing is already available in scope, then created span will be a child of the
        /// current span, otherwise a new root span will be created
        /// </summary>
        /// <returns>The created span(not yet started)</returns>
        ISpan NextSpan();

        /// <summary>
        /// Create a next span for the current Handler's context. If context is not available we raise an 
        /// exception. If tracing is already available in scope, then created span will be a child of the
        /// current span, otherwise a new root span will be created
        /// </summary>
        /// <param name="config">Configuration of the span we want to create. Check <see cref="ISpanConfig"/> for details</param>
        /// <returns>The created span(not yet started)</returns>
        ISpan NextSpan(ISpanConfig config);

        /// <summary>
        /// Create a next span for the <c>linkedObject</c>. If tracing is already available in scope, then created span will
        /// be a child of the current span, otherwise a new root span will be created
        /// </summary>
        /// <param name="linkedObject">Object associated with the span</param>
        /// <param name="config">Configuration of the span we want to create. Check <see cref="ISpanConfig"/> for details</param>
        /// <returns>Just created span(not yet started)</returns>
        ISpan NextSpan(object linkedObject, ISpanConfig config);

        /// <summary>
        /// Create a child span for the provided parent span
        /// </summary>
        /// <param name="parent">The parent span we are creating child for</param>
        /// <returns>Just created span(not yet started)</returns>
        ISpan ChildSpan(ISpan parent);

        /// <summary>
        /// Create a child span for the provided parent span
        /// </summary>
        /// <param name="parent">The parent span we are creating child for</param>
        /// <param name="config">Configuration of the span we want to create. Check <see cref="ISpanConfig"/> for details</param>
        /// <returns>Just created span(not yet started)</returns>
        ISpan ChildSpan(ISpan parent, ISpanConfig config);

        /// <summary>
        /// Fetch the current span associated with Handler's context. If there is no Handler's context in the
        /// current scope, then we return null. If there is a span already associated with the current handler's
        /// context then we return it. Otherwise we fetch SpanAdapter using CurrentSpan() from each TraceAdapter
        /// and create Span linked to them
        /// </summary>
        /// <returns>The current span associated with handler's context or null, if there is no context at all</returns>
        ISpan CurrentSpan();

        /// <summary>
        /// Fetch the current span associated with the <c>linkedObject</c>. If there is no associated span
        /// in the stack then we either create a new one, if <c>autoLink</c> is set to <c>true</c> or 
        /// we return <c>null</c> if set to <c>false</c>. If we have to create a Span then we fetch SpanAdapter
        /// using CurrentSpan() from each TraceAdapter and create Span linked to them
        /// </summary>
        /// <param name="linkedObject">Object associated with the span</param>
        /// <param name="autoLink">If true and there is no link to the current object of any span then we fetch the current span from
        /// the handler's context and link it to the current object</param>
        /// <returns>The current span linked to the object or null, if link is missing and auto-creation was disabled</returns>
        ISpan CurrentSpan(object linkedObject, bool autoLink = true);

        /// <summary>
        /// Create an empty span using the provided configuration
        /// </summary>
        /// <returns>Just created span(not yet started)</returns>
        ISpan CreateSpan();

        /// <summary>
        /// Create an empty span using the provided configuration
        /// </summary>
        /// <param name="config">Configuration of the span we want to create. Check <see cref="ISpanConfig"/> for details</param>
        /// <returns>Just created span(not yet started)</returns>
        ISpan CreateSpan(ISpanConfig config);

        /// <summary>
        /// Set the provided span in scope and make it current. This method activates the current span and make it
        /// current for the current Handler's context. Additionally this method calls <c>SpanAdapter.SetInScope()</c>
        /// for each attached adapter, wchih may perform some extra actions specific for tracing implementations, for
        /// example, activating span on the current thread.This method also starts span, if not yet started
        /// </summary>
        /// <param name="span">The span to set in scope</param>
        /// <returns>Created <see cref="ISpanScope"/> object which can be used with <c>try</c>-with-resources blocks</returns>
        ISpanScope SetInScope(ISpan span);

        /// <summary>
        /// Add span to the current handler's context stack of spans. If stack does not exist we create it
        /// </summary>
        /// <param name="span">Span to save</param>
        void SaveSpanToStack(ISpan span);

        /// <summary>
        /// Remove span from stack associated with the current handler's context
        /// </summary>
        /// <param name="span">The span to remove from the stack</param>
        void RemoveSpanFromStack(ISpan span);
    }
}
