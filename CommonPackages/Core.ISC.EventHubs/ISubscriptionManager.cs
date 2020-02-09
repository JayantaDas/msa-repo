using System;
using System.Collections.Generic;
using System.Text;
using Core.ISC.Messaging;


namespace Core.ISC.EventHubs
{
    public interface ISubscriptionManager
    {

        public bool IsEmpty { get; }      

        void AddSubscription<T, TH>(string eventUniqueID) where T : EventMetaData where TH : IEventHandler<T>;
        void AddSubscription<TH>(string eventUniqueID) where TH : IEventHandler;

        void RemoveSubscription<TH>(string eventUniqueID) where TH : IEventHandler;

        bool HasSubscriptionsForEvent(string eventUniqueID);
        
        public IReadOnlyDictionary<Subscription, Type> GetHandlersForEventId(string eventUniqueID);
        string GetEventName<T>();
        void Clear();
        
    }
}
