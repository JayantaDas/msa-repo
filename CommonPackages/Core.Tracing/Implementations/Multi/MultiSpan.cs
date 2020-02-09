using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Core.Tracing
{
    public class MultiSpan : ISpan
    {
        private readonly ILogger<MultiSpan> logger;

        /// <inheritdoc/>
        public ISpanBuilder SpanBuilder { get; private set; }

        /// <inheritdoc/>
        public ITracer Tracer { get; private set; }

        internal List<ISpan> Spans { get; private set; } = new List<ISpan>();

        private SpanState state = SpanState.None;

        ISpanContext spanContext;

        public MultiSpan(ILogger<MultiSpan> logger, ISpanBuilder spanBuilder, ITracer tracer, ISpanContext spanContext)
        {
            this.logger = logger;
            this.SpanBuilder = spanBuilder;
            this.Tracer = tracer;
            this.spanContext = spanContext;
        }

        /// <inheritdoc/>
        public void AddAdapter(ISpan span)
        {
            if (span != null)
            {
                this.Spans.Add(span);
            }
            else
            {
                logger.LogError(new TracingRuntimeException("Can't add 'null' span adapter"), "Null");
            }
        }

        /// <inheritdoc/>
        public ISpan SetName(string name)
        {
            try
            {
                this.Spans.ForEach(adapter => adapter.SetName(name));
            }
            catch (Exception e)
            {
                logger.LogWarning($"Span error occurred for name() method (name: {name}",  e);
            }
            return this;
        }

        /// <inheritdoc/>
        public ISpan AddTag(string key, string value)
        {
            try
            {
                this.Spans.ForEach(adapter => adapter.AddTag(key, value));
            }
            catch (Exception e)
            {
                logger.LogWarning($"Span error occurred for tag() method (key: {key}, value: {value})", e);
            }
            return this;
        }

        /// <inheritdoc/>
        public void AddPropagationHeaders()
        {
            try
            {
                this.Spans.ForEach(adapter => adapter.AddPropagationHeaders());
            }
            catch (Exception e)
            {
                logger.LogWarning("Span error occurred for addPropagationHeaders() method", e);
            }
        }

        /// <inheritdoc/>
        public ISpan AddError(Exception ex)
        {
            try
            {
                this.Spans.ForEach(adapter => adapter.AddError(ex));
                if (this.Tracer.IsStackTraceOnError)
                {
                    string stack = ex.StackTrace;
                    this.Spans.ForEach(adapter => adapter.AddTag("error.stack", stack));
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(string.Format("Span error occurred for error() method (ex.message: {0})", ex.Message), e);
            }
            return this;
        }

        /// <inheritdoc/>
        public ISpan SetInScope()
        {
            try
            {
                this.Spans.ForEach(adapter => adapter.SetInScope());
            }
            catch (Exception e)
            {
                logger.LogWarning("Span error occurred for setInScope() method", e);
            }
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
                try
                {
                    this.Spans.ForEach(adapter => adapter.Start());
                }
                catch (Exception e)
                {
                    logger.LogWarning("Span error occurred for start() method", e);
                }
            }
            return this;
        }

        /// <inheritdoc/>
        public void Finish()
        {
            // We finish span only if it was not finished yet
            if (!state.Equals(SpanState.Finished))
            {
                try
                {
                    this.Spans.ForEach(adapter => adapter.Finish());
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Span error occurred for finish() method");
                }
                this.state = SpanState.Finished;
            }
            this.Tracer.RemoveSpanFromStack(this);
        }
    }
}
