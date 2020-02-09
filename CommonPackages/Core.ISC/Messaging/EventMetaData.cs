using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Core.ISC.Messaging
{
    /// <summary>
    /// All events published and consumed must implement this base class
    /// </summary>
    public class EventMetaData
    {
        public EventMetaData(Guid Id, DateTime creationDate,string data, string eventUniqueID)
        {
            this.Id = Id;
            CreationDate = creationDate;
            this.EventUniqueID = eventUniqueID;
            this.data = data;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public string data { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public string EventUniqueID { get; private set; }

    }
}
