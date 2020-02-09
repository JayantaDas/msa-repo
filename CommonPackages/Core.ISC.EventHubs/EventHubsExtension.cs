using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Core.ISC.Messaging;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;

namespace Core.ISC.EventHubs
{
    public static class EventHubsExtensions
    {
        public static IServiceCollection AddISCMessaging(this IServiceCollection serviceCollection, IConfiguration configuration,List<Type> handlers)
        {

            var connectionString = configuration[EventHubsKeyConstants.connectionString] ?? EventHubsValueConstants.connectionString;
            serviceCollection.AddSingleton<ISubscriptionManager, InMemorySubscriptionManager>();
            serviceCollection.AddSingleton<IEventMessaging, EventMessaging>(sp =>
            {

                var subscriptionManager = sp.GetRequiredService<ISubscriptionManager>();
                return new EventMessaging(subscriptionManager, connectionString);
            });
            serviceCollection.AddTransient<IEventProcessor, BasicEventProcessor>(sp =>
            {
                return new BasicEventProcessor(sp);
            }
            );
            foreach(var handler in handlers)
            {
                serviceCollection.AddTransient(handler);
            }
            return serviceCollection;
        }

        public static IServiceProvider UseISCMessaging(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var connectionString = configuration[EventHubsKeyConstants.connectionString] ?? EventHubsValueConstants.connectionString;
            var consumerGroupName = configuration[EventHubsKeyConstants.consumerGroupName] ?? EventHubsValueConstants.consumerGroupName;
            var eventHubName = configuration[EventHubsKeyConstants.eventHubsName] ?? EventHubsValueConstants.eventHubName;
            var storageAccKey = configuration[EventHubsKeyConstants.storageAccountKey] ?? EventHubsValueConstants.storageAccountKey;
            var storageAccName = configuration[EventHubsKeyConstants.storageAccountName] ?? EventHubsValueConstants.storageAccountName;
            var storageConnString = $"DefaultEndpointsProtocol=https;AccountName={storageAccName};AccountKey={storageAccKey};EndpointSuffix=core.windows.net";
            var storageContainerName = configuration[EventHubsKeyConstants.storageContainerName] ?? EventHubsValueConstants.storageContainerName;

            var eventProcessorHost = new EventProcessorHost(EventHubsValueConstants.eventProcessorHostName, eventHubName, consumerGroupName, connectionString, storageConnString, storageContainerName);
            eventProcessorHost.RegisterEventProcessorFactoryAsync(new EventProcessorFactory(serviceProvider));
            return serviceProvider;
        }
    }
}
