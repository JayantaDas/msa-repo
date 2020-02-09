using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ISC.Messaging
{
    /// <summary>
    /// Interface used to handle the events with mapped native event data object. 
    /// All event handlers must inherit this interface.
    /// </summary>
    /// <typeparam name="TEventData"> The mapped native event. </typeparam>
    public interface IEventHandler<in TEventData> : IEventHandler
          where TEventData : EventMetaData
    {
        Task Handle(TEventData @event);
    }

    /// <summary>
    /// Interface used to handle the events with serialzed data passed as string.
    /// All event handlers must inherit thus interface.
    /// </summary>
    public interface IEventHandler
    {
        Task Handle(string serializedData);
    }
}
