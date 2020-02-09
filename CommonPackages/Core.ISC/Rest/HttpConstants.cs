using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ISC.Rest
{
    public static class HttpConstants
    {
        public const string DefaultContentType = "text/plain";
        public const string ContentTypeHeader = "Content-Type";
        public const string AuthorizationHeader = "Authorization";
        public const string JsonContentType = "application/json";

    }

    public static class RestConstants
    {
        public const string defaultNamedClient = "RestClient";
    }
}
