using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Core.Tracing
{
    public class SingleTracer : ITracer
    {
        private readonly ILogger<SingleTracer> logger;
        private readonly IServiceProvider serviceProvider;

        private ConditionalWeakTable<object, List<ISpan>> linkedSpans = new ConditionalWeakTable<object, List<ISpan>>();

        /// <inheritdoc/>
        public ISpanBuilder SpanBuilder { get; set; }

        /// <inheritdoc/>
        public ISpanConfig DefaultSpanConfig { get; set; }

        /// <inheritdoc/>
        public ISpanContext SpanContext { get; set; }

        /// <inheritdoc/>
        public bool IsStackTraceOnError { get; set; }

        public SingleTracer(ILogger<SingleTracer> logger, IServiceProvider serviceProvider, ISpanContext spanContext)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.SpanContext = spanContext;
        }

        /// <inheritdoc/>
        public void AddAdapter(ISpanBuilder spanBuilder)
        {
            logger.LogError(new TracingRuntimeException("Cannot add multiple ISpanBuilders"), "Not Implemented");
        }

        /// <inheritdoc/>
        public ISpan ChildSpan(ISpan parent)
        {
            return ChildSpan(parent, SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan ChildSpan(ISpan parent, ISpanConfig config)
        {
            return this.SpanBuilder.ChildSpan(parent, config);
        }

        /// <inheritdoc/>
        public ISpan CreateSpan()
        {
            return CreateSpan(SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan CreateSpan(ISpanConfig config)
        {
            return this.SpanBuilder.CreateSpan(config);
        }

        /// <inheritdoc/>
        public ISpan CurrentSpan()
        {
            return CurrentSpan(this.SpanContext);
        }

        /// <inheritdoc/>
        public ISpan CurrentSpan(object linkedObject, bool autoLink = true)
        {
            List<ISpan> spans = linkedSpans.GetOrCreateValue(linkedObject);
            ISpan current = spans.FirstOrDefault();
            if (current == null && autoLink)
            {
                ISpan span = this.CreateSpan();
                spans.Insert(0, span);
                return span;
            }
            else
            {
                return current;
            }
        }

        /// <inheritdoc/>
        public ISpan NextSpan()
        {
            return NextSpan(SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan NextSpan(ISpanConfig config)
        {
            return NextSpan(this.SpanContext, config);
        }

        /// <inheritdoc/>
        public ISpan NextSpan(object linkedObject, ISpanConfig config)
        {
            ISpan currentSpan = this.CurrentSpan(linkedObject);
            if (currentSpan != null)
            {
                return this.ChildSpan(currentSpan, config);
            }
            else
            {
                ISpan span = this.SpanBuilder.CreateSpan(config);
                return span;
            }
        }

        /// <inheritdoc/>
        public void RemoveSpanFromStack(ISpan span)
        {
            linkedSpans.GetOrCreateValue(this.SpanContext).Remove(span);
        }

        /// <inheritdoc/>
        public void SaveSpanToStack(ISpan span)
        {
            List<ISpan> stack = linkedSpans.GetOrCreateValue(this.SpanContext);
            if (!stack.Contains(span))
            {
                stack.Insert(0, span);
            }
        }

        /// <inheritdoc/>
        public ISpanScope SetInScope(ISpan span)
        {
            this.SaveSpanToStack(span);
            span.Start();
            span.SetInScope();
            ISpanScope spanScope = this.serviceProvider?.GetRequiredService<ISpanScope>();
            if (spanScope != null)
                spanScope.Span = span;
            return spanScope;
        }
    }
}
