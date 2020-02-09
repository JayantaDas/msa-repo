namespace Core.Tracing
{
    public interface ISpanBuilder
    {
        /// <summary>
        /// Create an empty span using the provided configuration
        /// </summary>
        /// <returns>Just created span(not yet started)</returns>
        ISpan CreateSpan();

        /// <summary>
        /// Create a Span using custom configuration. Usually used by adapters
        /// and binders to auto-create spans for incoming requests / messages
        /// or outgoing requests, etc.
        /// </summary>
        /// <param name="config">An instance of Span configuration. May contain:
        /// - [sourceLocation - which can be either <c>Internal</c>, <c>Incoming</c> or <c>Outgoing</c>]
        /// - [sourceType - for example, <c>REST</c>, <c>MESSAGE</c>, <c>GRPC</c>]
        /// - [sourceName - which can be a Topic or Queue name for messages]
        /// - [headers map - for example, containing received HTTP or Message headers]
        /// - [linkedObject - to link this span to and dispose when its garbage collected]
        /// Check <seealso cref="ISpanConfig"/> for more details.</param>
        /// <returns>Just creates Span (does not start it)</returns>
        ISpan CreateSpan(ISpanConfig config);

        /// <summary>
        /// Create a child span for the provided parent span
        /// </summary>
        /// <param name="parent">The parent span we are creating child for</param>
        /// <returns>Just created span(not yet started)</returns>
        ISpan ChildSpan(ISpan parent);

        /// <summary>
        /// Create child span under the provided span
        /// </summary>
        /// <param name="parentSpan">Link to the parent span</param>
        /// <param name="config">An instance of Span configuration. May contain:
        /// - [sourceLocation - which can be either <c>Internal</c>, <c>Incoming</c> or <c>Outgoing</c>]
        /// - [sourceType - for example, <c>REST</c>, <c>MESSAGE</c>, <c>GRPC</c>]
        /// - [sourceName - which can be a Topic or Queue name for messages]
        /// - [headers map - for example, containing received HTTP or Message headers]
        /// - [linkedObject - to link this span to and dispose when its garbage collected]
        /// Check <see cref="ISpanConfig"/> for more details.</param>
        /// <returns>Just creates Span (does not start it)</returns>
        ISpan ChildSpan(ISpan parentSpan, ISpanConfig config);

        /// <summary>
        /// Get current span from the system context. Call to this method is 
        /// safe because it will return an empty span if there is no current span
        /// </summary>
        ISpan CurrentSpan { get; }
    }
}
