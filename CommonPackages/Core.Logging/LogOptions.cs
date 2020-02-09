namespace Core.Logging
{
    public class LogOptions
    {
        public string CorrelationHeader { get; set; } = LoggerConstants.DEFAULT_CORRELATION_HEADER;
        public string ParentId { get; set; } = LoggerConstants.DEFAULT_PARENT_ID;

        public bool IncludeInResponse { get; set; } = true;

        public bool UpdateTraceIdentifier { get; set; } = true;

        public bool UseGuidForCorrelationId { get; set; } = true;
    }
}
