using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using System;
using Serilog.Sinks.RollingFile;

namespace Core.Logging.Serilog
{
    public static class WebhostBuilderExtensions
    {
        public static void AddLoggingService(this IServiceCollection services,
                                             IConfiguration configuration,
                                             bool preserveStaticLogger = false)
        {
            try
            {
                var loggingSubType = configuration.GetSection("LoggingType").GetSection("LoggingSubType").Value;

                if (!string.IsNullOrEmpty(loggingSubType))
                {
                    if (loggingSubType == "ApplicationInsights")
                    {
                        UseAppInsights(services, configuration);
                    }
                    else
                    {
                        UseAspNetLog(services, configuration);
                    }
                }
            }
            catch (Exception e)
            {
                //Exception
            }
        }

        /*public static void UseAspNetLog(IServiceCollection services,
                                        string logSettingFileName,
                                        bool preserveStaticLogger = false)
        {
            try
            {
                var serilogConfig = new ConfigurationBuilder().AddJsonFile(logSettingFileName).Build();

                Action<IServiceProvider, IServiceCollection, LoggerConfiguration> configureLogger = (provider, ctx, config) =>
                {
                    var options = new LogOptions()
                    {
                        CorrelationHeader = serilogConfig[LoggerConstants.CONFIG_CORRELATION_HEADER] ?? LoggerConstants.DEFAULT_CORRELATION_HEADER
                    };
                    config.ReadFrom.Configuration(serilogConfig).Enrich.WithCorrelationId(provider, Options.Create(options));
                };

                services.AddApplicationLogger();
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                var provider = services.BuildServiceProvider();

                services.AddSingleton<CorrelationIdEnricher>();

                LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
                configureLogger(provider, services, loggerConfiguration);

                Logger logger = loggerConfiguration.CreateLogger();
                if (preserveStaticLogger)
                {
                    services.AddSingleton(services => (ILoggerFactory)new SerilogLoggerFactory(logger, true));
                }
                else
                {
                    Log.Logger = logger;
                    services.AddSingleton(services => (ILoggerFactory)new SerilogLoggerFactory(null, true));
                }
            }
            catch(Exception e)
            {
                //Exception
            }
        }*/

        public static void UseAppInsights(IServiceCollection services,
                                          IConfiguration configuration,
                                          bool preserveStaticLogger = false)      
        {
            try
            {
                services.AddApplicationLogger();
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                //var provider = collection.BuildServiceProvider();
                services.AddSingleton<CorrelationIdEnricher>();
                services.AddApplicationInsightsTelemetry(configuration);
                services.AddSingleton<ILoggerFactory>(provider =>
                {
                    TelemetryConfiguration telemetryConfiguration = provider.GetRequiredService<TelemetryConfiguration>();

                    var loggerConfiguration = new LoggerConfiguration();
                    var options = new LogOptions()
                    {
                        CorrelationHeader = configuration[LoggerConstants.CONFIG_CORRELATION_HEADER] ?? LoggerConstants.DEFAULT_CORRELATION_HEADER,
                        ParentId = configuration[LoggerConstants.CONFIG_CORRELATION_HEADER] ?? LoggerConstants.DEFAULT_PARENT_ID
                    };

                    loggerConfiguration.ReadFrom.Configuration(configuration)
                    .Enrich.WithCorrelationId(provider, Options.Create(options))
                    //.Enrich.FromLogContext()
                    .WriteTo.ApplicationInsights(telemetryConfiguration.InstrumentationKey, new OperationTelemetryConverter(Options.Create(options)));

                    Logger logger = loggerConfiguration.CreateLogger();

                    Log.Logger = logger;
                    var factory = new SerilogLoggerFactory(logger, true);
                    return factory;
                });
            }
            catch(Exception e)
            {
                //Exception
            }
        }

        public static void UseAspNetLog(IServiceCollection services,
                                      IConfiguration configuration,
                                      bool preserveStaticLogger = false)
        {
            // define BASEDIR which is defined in serilogfilelog.json
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);

            try
            {
                services.AddApplicationLogger();
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddSingleton<ILoggerFactory>(provider =>
                {
                    var loggerConfiguration = new LoggerConfiguration();
                    var options = new LogOptions()
                    {
                        CorrelationHeader = configuration[LoggerConstants.CONFIG_CORRELATION_HEADER] ?? LoggerConstants.DEFAULT_CORRELATION_HEADER
                    };

                    loggerConfiguration.ReadFrom.Configuration(configuration)
                    .Enrich.WithCorrelationId(provider, Options.Create(options));

                    Logger logger = loggerConfiguration.CreateLogger();

                    Log.Logger = logger;
                    var factory = new SerilogLoggerFactory(logger, true);
                    return factory;
                });
            }
            catch(Exception e)
            {
                //Exception
            }
        }
    }
}
