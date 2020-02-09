using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Core.Common
{
    public interface IProtocolContext
    {
        /// <summary>
        /// Gets Dictionary containing all headers present in the context
        /// </summary>
        Dictionary<string, StringValues> Headers { get; }

        /// <summary>
        /// Gets the correlation-id discovered or generated for the protocol handler
        /// </summary>
        string CorrelationId { get; }
    }
}
