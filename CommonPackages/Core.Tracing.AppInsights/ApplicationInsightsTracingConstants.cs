namespace Core.Tracing.ApplicationInsights
{
    public static class ApplicationInsightsTracingConstants
    {
        public const string HTTP_HEADER_TRACEPARENT = "traceparent";
        public const string HTTP_HEADER_REQUESTID = "request-id";

        public const string MESSAGE_HEADER_AZURE_TRACEPARENT = "x-azure-applicationinsights-traceparent";
    }
}
