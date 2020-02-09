using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using Core.ISC.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Core.ISC.EventHubs.Test")]
namespace Core.ISC.EventHubs
{
    /// <summary>
    /// BasicEventProcessor class is used to receive the events from EventHubs and then process them.
    /// IserviceProvider is a dependency and is used by this class to fetch the subscriptions  
    /// </summary>
    class BasicEventProcessor : IEventProcessor
    {
        private readonly ISubscriptionManager subscriptionManager;
        private readonly IServiceProvider serviceProvider;
        public BasicEventProcessor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.subscriptionManager = serviceProvider.GetRequiredService<ISubscriptionManager>();
        }
        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {

                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(data);
                           
                Task.Run(async () =>
                {
                    var eventUniqueID = eventMetaData.EventUniqueID;
                    await ProcessEventAsync(eventUniqueID, data);
                });
            }

            return context.CheckpointAsync();
        }

        private async Task<bool> ProcessEventAsync(string eventUniqueID, string data)
        {
            var processed = false;
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            if (subscriptionManager.HasSubscriptionsForEvent(eventUniqueID))
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var subscriptions = subscriptionManager.GetHandlersForEventId(eventUniqueID);
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService(subscription.Key.HandlerType);
                        if (handler != null)
                        {
                            if (subscription.Value != null)
                            {
                                var eventData = JsonConvert.DeserializeObject(data, subscription.Value);
                                var concreteEventHandlerType = typeof(IEventHandler<>).MakeGenericType(subscription.Value);
                                await (Task)concreteEventHandlerType.GetMethod(EventHubsValueConstants.eventHandlerMethodName).Invoke(handler, new object[] { eventData });
                            }
                            else
                            {
                                await (handler as IEventHandler).Handle(data);
                            }
                        }
                    }
                }
                processed = true;
            }
            return processed;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            return Task.CompletedTask;
        }
        public Task OpenAsync(PartitionContext context)
        {
            return Task.FromResult<object>(null);
        }
        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
