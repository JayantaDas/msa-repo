using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Core.Tracing
{
    public class MultiTracer : ITracer
    {
        private readonly ILogger<MultiTracer> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly ISpanContext spanContext;

        private ConditionalWeakTable<object, List<ISpan>> linkedSpans = new ConditionalWeakTable<object, List<ISpan>>();

        /// <inheritdoc/>
        public ISpanBuilder SpanBuilder { get; set; }

        /// <inheritdoc/>
        public bool IsStackTraceOnError { get; set; }

        internal List<ISpanBuilder> SpanBuilders { get; private set; } = new List<ISpanBuilder>();

        public MultiTracer(ILogger<MultiTracer> logger, IServiceProvider serviceProvider, ISpanContext spanContext)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.spanContext = spanContext;
        }

        /// <inheritdoc/>
        public void AddAdapter(ISpanBuilder spanBuilder)
        {
            if (spanBuilder != null)
            {
                this.SpanBuilders.Add(spanBuilder);
            }
            else
            {
                logger.LogError(new TracingRuntimeException("Can't add 'null' span builder"), "Null Span Builder");
            }
        }

        /// <inheritdoc/>
        public ISpan ChildSpan(ISpan parent)
        {
            return ChildSpan(parent, SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan ChildSpan(ISpan parent, ISpanConfig config)
        {
            ISpan child = this.serviceProvider.GetRequiredService<ISpan>();
            this.SpanBuilders.ForEach(builder =>
            {
                try
                {
                    child.AddAdapter(builder.ChildSpan(parent, config));
                }
                catch (Exception e)
                {
                    logger.LogWarning($"Error occurred calling TracerAdapter#childSpan() for {builder.GetType().Name} : {e.Message}", e);
                }
            });
            return child;
        }

        /// <inheritdoc/>
        public ISpan CreateSpan()
        {
            return CreateSpan(SpanConfig.None);
        }

        /// <inheritdoc/>
        public ISpan CreateSpan(ISpanConfig config)
        {
            ISpan span = this.serviceProvider.GetRequiredService<ISpan>();
            this.SpanBuilders.ForEach(adapter =>
            {
                try
                {
                    span.AddAdapter(adapter.CreateSpan(config));
                }
                catch (Exception e)
                {
                    logger.LogWarning($"Error occurred calling createSpan() for {adapter.GetType().Name} : {e.Message}", e);
                }
            });
            return span;
        }

        /// <inheritdoc/>
        public ISpan CurrentSpan()
        {
            return CurrentSpan(this.spanContext);
        }

        /// <inheritdoc/>
        public ISpan CurrentSpan(object linkedObject, bool autoLink = true)
        {
            List<ISpan> spans = linkedSpans.GetOrCreateValue(linkedObject);
            ISpan current = spans.FirstOrDefault();
            if (current == null && autoLink)
            {
                ISpan span = this.serviceProvider.GetRequiredService<ISpan>();
                this.SpanBuilders.ForEach(adapter =>
                {
                    try
                    {
                        span.AddAdapter(adapter.CurrentSpan);
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning($"Error occurred calling currentSpan() for {adapter.GetType().Name} : {e.Message}", e);
                    }
                });
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
            return NextSpan(this.spanContext, config);
        }

        /// <inheritdoc/>
        public ISpan NextSpan(object linkedObject, ISpanConfig config)
        {
            ISpan currentSpan = this.CurrentSpan(linkedObject);
            if (currentSpan != null)
            {
                return ChildSpan(currentSpan, config);
            }
            else
            {
                ISpan span = this.serviceProvider.GetRequiredService<ISpan>();
                this.SpanBuilders.ForEach(adapter =>
                {
                    try
                    {
                        span.AddAdapter(adapter.CreateSpan(config));
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning($"Error occurred calling TracerAdapter-CreateSpan() for {adapter.GetType().Name} : {e.Message}", e);
                    }
                });
                return span;
            }
        }

        /// <inheritdoc/>
        public void RemoveSpanFromStack(ISpan span)
        {
            linkedSpans.GetOrCreateValue(this.spanContext).Remove(span);
        }

        /// <inheritdoc/>
        public void SaveSpanToStack(ISpan span)
        {
            List<ISpan> stack = linkedSpans.GetOrCreateValue(this.spanContext);
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
            ISpanScope spanScope = this.serviceProvider.GetRequiredService<ISpanScope>();
            spanScope.Span = span;
            return spanScope;
        }
    }
}
