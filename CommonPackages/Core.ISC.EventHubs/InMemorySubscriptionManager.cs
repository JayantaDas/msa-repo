using System;
using System.Linq;
using Core.ISC.Messaging;
using System.Collections.Generic;

namespace Core.ISC.EventHubs
{
    public class InMemorySubscriptionManager : ISubscriptionManager
    {

        private readonly Dictionary<string, Dictionary<Subscription, Type>> subscriptions;


        public event EventHandler<string> OnEventRemoved;

        public InMemorySubscriptionManager()
        {
            subscriptions = new Dictionary<string, Dictionary<Subscription, Type>>();
        }

        public bool IsEmpty => !subscriptions.Keys.Any();
        public void Clear() => subscriptions.Clear();


        public void AddSubscription<T, TH>(string eventUniqueID) where T : EventMetaData where TH : IEventHandler<T>
        {
            DoAddSubscription(eventUniqueID, typeof(TH), typeof(T));
        }

        public void AddSubscription<TH>(string eventUniqueID) where TH : IEventHandler
        {
            DoAddSubscription(eventUniqueID, typeof(TH));
        }

        public void RemoveSubscription<TH>(string eventUniqueID) where TH : IEventHandler
        {
            var subsToRemove = FindSubscriptionToRemove(eventUniqueID, typeof(TH));
            DoRemoveSubscription(eventUniqueID, subsToRemove);
        }

        public IReadOnlyDictionary<Subscription, Type> GetHandlersForEventId(string eventUniqueID) => subscriptions[eventUniqueID];

        public bool HasSubscriptionsForEvent(string eventUniqueID) => subscriptions.ContainsKey(eventUniqueID);

        public string GetEventName<T>()
        {
            return typeof(T).Name;
        }
        private Subscription FindSubscriptionToRemove(string eventUniqueID, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventUniqueID))
            {
                return null;
            }

            return subscriptions[eventUniqueID].SingleOrDefault(s => s.Key.HandlerType == Subscription.Typed(handlerType).HandlerType).Key;
        }
        private void DoRemoveSubscription(string eventUniqueID, Subscription subsToRemove)
        {
            if (subsToRemove != null)
            {

                subscriptions[eventUniqueID].Remove(subsToRemove);
                if (!subscriptions[eventUniqueID].Any())
                {
                    subscriptions.Remove(eventUniqueID);
                }
            }
        }

        private void DoAddSubscription(string eventUniqueID, Type handlerType, Type eventType = null)
        {
            if (!HasSubscriptionsForEvent(eventUniqueID))
            {
                subscriptions.Add(eventUniqueID, new Dictionary<Subscription, Type>());
            }

            if (subscriptions[eventUniqueID].Any(s => s.Key.HandlerType == Subscription.Typed(handlerType).HandlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventUniqueID}'", nameof(handlerType));
            }

            subscriptions[eventUniqueID].Add(Subscription.Typed(handlerType), eventType);
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }


    }
}
