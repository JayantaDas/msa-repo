using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Core.Common
{
    public class ProtocolContext : IProtocolContext
    {
        /// <summary>
        /// Gets Dictionary containing all headers present in the context
        /// </summary>
        public Dictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();

        /// <summary>
        /// Gets the correlation-id discovered or generated for the protocol handler
        /// </summary>
        public string CorrelationId { get; set; }
    }
}
