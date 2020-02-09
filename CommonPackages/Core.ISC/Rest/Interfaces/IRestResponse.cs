using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Core.ISC.Rest
{
    /// <summary>
    /// Interface which defines specific type of response expected from the HTTP Request
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRestResponse<TResponse>
    {
        int Code { get; }
        /// <summary>
        /// HTTP response status code.
        /// </summary>
        HttpStatusCode StatusCode { get; }
        TResponse Data { get; }
        Exception Exception { get; }
        HttpResponseMessage OriginalHttpResponseMessage { get; }
        object RawData { get; }
        bool IsError { get; }
        TimeSpan RequestTime { get; }
        long RequestTimeMs { get; }
    }
}
