using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tracing.ApplicationInsights
{
    public static class ApplicationInsightsTracingExtensions
    {
        public static void AddApplicationInsightsTracing(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<TelemetryClient>(_ =>
            {
                var client = new TelemetryClient(TelemetryConfiguration.CreateDefault());
                client.InstrumentationKey = config?["ApplicationInsights:InstrumentationKey"];
                return client;
            });
            services.AddTransient<ISpan, ApplicationInsightsSpan>();
            services.AddScoped<ISpanBuilder, ApplicationInsightsSpanBuilder>();
            services.AddTransient<ISpanConfig, SpanConfig>();
            services.AddScoped<ISpanContext, SpanContext>();
            services.AddTransient<ISpanScope, SpanScope>();
            services.AddScoped<ITracer, SingleTracer>();
        }

        public static void AddApplicationInsightsTracing(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseMiddleware<ApplicationInsightsTracingMiddleware>();
        }
    }
}
