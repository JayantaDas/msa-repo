using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace Core.Logging.Serilog
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly LogOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMiddleware"/> class.
        /// Creates a new instance of the Application Log Middleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">The configuration options.</param>
        public LoggerMiddleware(RequestDelegate next, IOptions<LogOptions> options)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (LogContext.PushProperty(options.CorrelationHeader, context.TraceIdentifier))
            {
                await this.next?.Invoke(context);
            }
        }
    }
}
