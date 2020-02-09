using CorrelationId;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Logging.Serilog
{
    public static class LoggerServiceExtensions
    {
        public static IServiceCollection AddApplicationLogger(this IServiceCollection serviceCollection, LogOptions logOptions = null)
        {
            serviceCollection.AddCorrelationId();
            return serviceCollection;
        }
    }
}
