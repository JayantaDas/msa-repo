using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ISC.Messaging
{
    public interface IEventMessaging
    {
        Task Publish<T>(T customEvent) where T : EventMetaData;
        /// <summary>
        /// Subscribe for events by passing the eventUniqueID
        /// </summary>
        /// <typeparam name="TH"> Event Handler that will be fired on receiving the event </typeparam>
        /// <param name="eventUniqueID"></param>
        void Subscribe<TH>(string eventUniqueID) where TH : IEventHandler;
        /// <summary>
        /// Subscribe for events by passing the eventUniqueID.
        /// </summary>
        /// <typeparam name="T"> The event type that will be used to desearliaze the event data received </typeparam>
        /// <typeparam name="TH"> Event Handler that will be fired on receiving the event </typeparam>
        /// <param name="eventUniqueID"></param>
        void Subscribe<T, TH>(string eventUniqueID) where T : EventMetaData where TH : IEventHandler<T>;
        void Unsubscribe<TH>(string eventUniqueID) where TH : IEventHandler;

    }
}
