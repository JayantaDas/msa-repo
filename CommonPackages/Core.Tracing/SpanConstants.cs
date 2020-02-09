namespace Core.Tracing
{
    public class SpanConstants
    {
        public const string TAG_REQUEST_TARGET_HOST = "core.request.target.host";
        public const string TAG_REQUEST_TARGET_PORT = "core.request.target.port";
        public const string TAG_REQUEST_METHOD = "http.method";
        public const string TAG_REQUEST_PATH = "http.path";
        public const string TAG_REQUEST_URL = "core.request.url";
        public const string TAG_RESPONSE_CODE = "core.response.code";
        public const string TAG_RESPONSE_SUCCESS = "core.response.success";
        public const string TAG_HANDLER_NAME = "core.handler.name";
        public const string TAG_HANDLER_CLASSNAME = "core.handler.className";

        public const string HEADER_TRACEPARENT = "traceparent";
        public const string HEADER_REQUESTID = "request-id";
        public const string HEADER_AZURE_TRACEPARENT = "x-azure-applicationinsights-traceparent";
        public const string HEADER_TRACE_ID = "x-traceid";
        public const string HEADER_USER_AGENT = "User-Agent";
    }
}
