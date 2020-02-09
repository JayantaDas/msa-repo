using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Core.Tracing
{
    //class SingleSpan
    public class SingleSpan : ISpan
    {
        private readonly ILogger<SingleSpan> logger;

        /// <inheritdoc/>
        public ISpanBuilder SpanBuilder { get; private set; }

        /// <inheritdoc/>
        public ITracer Tracer { get; private set; }

        private SpanState state = SpanState.None;

        private string name;
        private Dictionary<string, string> tags = new Dictionary<string, string>();
        private Dictionary<string, StringValues> headers;
        private List<Exception> errors = new List<Exception>();

        ISpanContext spanContext;

        public SingleSpan(ILogger<SingleSpan> logger, ISpanBuilder spanBuilder, ITracer tracer, ISpanContext spanContext)
        {
            this.logger = logger;
            this.SpanBuilder = spanBuilder;
            this.Tracer = tracer;
            this.spanContext = spanContext;
        }

        /// <inheritdoc/>
        public void AddAdapter(ISpan span)
        {
            logger.LogError(new TracingRuntimeException("Can't add 'null' span adapter"), "Null");
        }

        /// <inheritdoc/>
        public ISpan SetName(string name)
        {
            this.name = name;
            return this;
        }

        /// <inheritdoc/>
        public ISpan AddTag(string key, string value)
        {
            if (tags.ContainsKey(key))
                tags.Add(key, value);
            else
                tags[key] = value;

            return this;
        }

        /// <inheritdoc/>
        public void AddPropagationHeaders()
        {
            // TO DO
        }

        /// <inheritdoc/>
        public ISpan AddError(Exception ex)
        {
            try
            {
                this.errors.Add(ex);
                if (this.Tracer.IsStackTraceOnError)
                {
                    string stack = ex.StackTrace;
                    this.AddTag("error.stack", stack);
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Span error occurred for error() method (ex.message: {ex.Message})");
            }
            return this;
        }

        /// <inheritdoc/>
        public ISpan SetInScope()
        {
            // Do Nothing
            this.Tracer.SaveSpanToStack(this);
            return this;
        }

        /// <inheritdoc/>
        public ISpan ChildSpan()
        {
            return this.Tracer.ChildSpan(this, SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan Start()
        {
            // We start span only if it was not started or finished yet
            if (state.Equals(SpanState.None))
            {
                this.state = SpanState.Started;
                this.Tracer.SaveSpanToStack(this);
            }
            return this;
        }

        /// <inheritdoc/>
        public void Finish()
        {
            // We finish span only if it was not finished yet
            if (!state.Equals(SpanState.Finished))
            {   
                this.state = SpanState.Finished;
            }
            this.Tracer.RemoveSpanFromStack(this);
        }
    }
}
