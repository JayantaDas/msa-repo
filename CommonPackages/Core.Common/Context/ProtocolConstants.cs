using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common
{
    public static class ProtocolConstants
    {
        public const string HEADER_USER_AGENT = "User-Agent";
        public const string HEADER_REQUEST_ID = "request-id";

        public const string HEADER_CORRELATION_ID = "x-correlation-id";

        public static HashSet<string> INCOMING_HEADERS { get; private set; } = new HashSet<string>();

        public static HashSet<string> OUTGOING_HEADERS { get; private set; } = new HashSet<string>();

        static ProtocolConstants()
        {
            INCOMING_HEADERS.Add(HEADER_USER_AGENT);
            INCOMING_HEADERS.Add(HEADER_REQUEST_ID);
            INCOMING_HEADERS.Add(HEADER_CORRELATION_ID);

            OUTGOING_HEADERS.Add(HEADER_REQUEST_ID);
            OUTGOING_HEADERS.Add(HEADER_CORRELATION_ID);
        }
    }
}
