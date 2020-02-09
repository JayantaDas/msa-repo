using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Core.ISC.Rest
{
    public class RestResponse<TResponse> : IRestResponse<TResponse>
    {
        public TResponse Data { get; internal set; }
        public int Code { get; internal set; }
        public HttpStatusCode StatusCode { get; internal set; }
        public Exception Exception { get; internal set; }
        public HttpResponseMessage OriginalHttpResponseMessage { get; internal set; }
        public object RawData { get; internal set; }
        public bool IsError { get; internal set; }
        public TimeSpan RequestTime { get; internal set; }
        public long RequestTimeMs { get; internal set; }
    }
}
