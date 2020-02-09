using System;
using System.Collections.Generic;
using System.Text;
using Core.ISC.Messaging;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace Core.ISC.EventHubs
{
    public class EventMessaging : IEventMessaging
    {
        public ISubscriptionManager subscriptionManager;
        public EventHubClient eventHubClient;
        public EventMessaging(ISubscriptionManager subscriptionManager, string connectionString)
        {
            this.subscriptionManager = subscriptionManager;
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
        }

        public async Task Publish<T>(T CustomEvent) where T : EventMetaData

        {
            if (CustomEvent == null) throw new ArgumentNullException(nameof(CustomEvent));
            var serializedEvent = JsonConvert.SerializeObject(CustomEvent);
            var eventBytes = Encoding.UTF8.GetBytes(serializedEvent);
            var eventData = new EventData(eventBytes);
            await eventHubClient.SendAsync(eventData);

        }

        public void Subscribe<T, TH>(string eventUniqueID) where T : EventMetaData where TH : IEventHandler<T>
        {
            subscriptionManager.AddSubscription<T, TH>(eventUniqueID);
        }

        public void Subscribe<TH>(string eventUniqueID) where TH : IEventHandler
        {
            subscriptionManager.AddSubscription<TH>(eventUniqueID);
        }

        public void Unsubscribe<TH>(string eventUniqueID) where TH : IEventHandler
        {
            subscriptionManager.RemoveSubscription<TH>(eventUniqueID);
        }

    }
}
