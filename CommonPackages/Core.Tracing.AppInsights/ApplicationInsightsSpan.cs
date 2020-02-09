using Core.Common;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Core.Tracing.ApplicationInsights
{
    public class ApplicationInsightsSpan : ISpan
    {
        private const string PIPE = "|";
        private const string DOT = ".";

        private const string MESSAGE_CREATE_SPAN = "Creating Azure App Insights span for : ";

        private static TaskFactory tracker = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(5));

        private static Random random = new Random();

        private readonly ILogger<ApplicationInsightsSpan> logger;

        private readonly TelemetryClient telemetryClient;

        public string Uuid { get; private set; }
        
        public string OperationName { get; private set; }
        
        public string OperationId { get; private set; }
                
        private Interlock<bool> finished = false;

        //private TelemetryContext context;
        private RequestTelemetry requestTelemetry;
        private DependencyTelemetry remoteDependencyTelemetry;
        private bool remoteDependencyTelemetryForMessage = false;
        private DateTimeOffset startTime;
        private string tagsRemoteTargetHost;
        private string tagsRemoteTargetPort;
        private RequestTelemetryNameComponents requestTelemetryNameComponents = new RequestTelemetryNameComponents();

        /// <inheritdoc/>
        public ISpanBuilder SpanBuilder { get; private set; }

        /// <inheritdoc/>
        public ITracer Tracer { get; private set; }

        private ISpanContext spanContext;

        public ApplicationInsightsSpan(ILogger<ApplicationInsightsSpan> logger, ISpanBuilder spanBuilder, ITracer tracer, ISpanContext spanContext, TelemetryClient telemetryClient)
        {
            this.logger = logger;
            this.SpanBuilder = spanBuilder;
            this.Tracer = tracer;
            this.spanContext = spanContext;
            this.telemetryClient = telemetryClient;

            logger.LogDebug(MESSAGE_CREATE_SPAN + "no args");
        }

        public void AddAdapter(ISpan span)
        {
            logger.LogError(new NotImplementedException(), "Not for this");
        }

        /// <summary>
        /// Create Azure AppInsights child telemetry  linked to the parent telemetry		
        /// </summary>
        /// <param name="parentId">Id of the parent telemetry</param>
        /// <param name="operationId">Id of the root (operation)</param>
        private void InitSpan(string parentId, string operationId)
        {
            var timeNow = DateTimeOffset.UtcNow;
            logger.LogDebug(MESSAGE_CREATE_SPAN + "empty");

            //context = new RequestTelemetryContext(new Date().getTime());
            //requestTelemetry = context.getHttpRequestTelemetry();
            requestTelemetry = telemetryClient.StartOperation<RequestTelemetry>("Normal").Telemetry;
            this.OperationId = operationId;
            Uuid = operationId + DOT + random.Next().ToString("X");
            requestTelemetry.Context.Operation.ParentId = parentId;
            requestTelemetry.Id = PIPE + Uuid + DOT;
            requestTelemetry.Context.Operation.Id = operationId;
            requestTelemetry.Context.Operation.Name = Uuid;
            // requestTelemetry.AllowAgentToOverrideName=true;
            requestTelemetry.Timestamp = timeNow;
        }

        /// <summary>
        /// Create Azure AppInsights span for Incomming HTTP Requesrt using received headers
        /// </summary>
        /// <param name="headers">Received HTTP headers</param>
        /// <returns>Application Insights Span Adapter</returns>
        public void InitForIncomingRest()
        {
            var headers = this.spanContext.Headers;
            var timeNow = DateTimeOffset.UtcNow;
            logger.LogDebug(MESSAGE_CREATE_SPAN + "incoming rest, " + headers);

            //span.context = new RequestTelemetryContext(new Date().getTime());
            //span.requestTelemetry = span.context.getHttpRequestTelemetry();
            requestTelemetry = telemetryClient.StartOperation<RequestTelemetry>("Incoming REST").Telemetry;
            this.Uuid = ActivitySpanId.CreateRandom().ToHexString(); // LocalStringsUtils.generateRandomId(true);
            string parentId = FetchParentId();
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                this.requestTelemetry.Context.Operation.ParentId = parentId;
            }

            this.OperationId = this.Uuid;
            this.requestTelemetry.Id = PIPE + this.Uuid + DOT;
            this.requestTelemetry.Context.Operation.Id = this.OperationId;
            this.requestTelemetry.Context.Operation.Name = this.Uuid;
            //TODO: Check if User-Agent properly reported
            if (headers.ContainsKey(SpanConstants.HEADER_USER_AGENT))
                this.requestTelemetry.Context.User.UserAgent = headers[SpanConstants.HEADER_USER_AGENT];
            //TODO: make sure we aet name from ReactiveWeb code
            //this.name(request.getMethodValue() + " " + request.getURI().getPath());
            //TODOL make sure we set TARGET_URL from ReactiveWeb code
            //		try {
            //			requestTelemetry.setUrl(request.getURI().toString());
            //		}catch (Exception e) {
            //			LOGGER.error("Can't register request URL", e);
            //		}
            //TODO: uncomment after 2.5.0 version of Azure App Insights is released
            //requestTelemetry.setAllowAgentToOverrideName(true);
            this.requestTelemetry.Timestamp = timeNow;
        }

        /// <summary>
        /// Creates Azure AppInsights span for Incomming Message, using channel name and received headers
        /// </summary>
        /// <param name="messageChannelName">String representing logical message channel name</param>
        /// <param name="headers">Received Message headers</param>
        /// <returns>Application Insights Span Adapter</returns>
        public void InitForIncomingMessage(string messageChannelName)
        {
            var headers = this.spanContext.Headers;
            var timeNow = DateTimeOffset.UtcNow;
            logger.LogDebug(MESSAGE_CREATE_SPAN + messageChannelName + ", " + headers);

            string parentId = FetchParentId();

            //span.context = new RequestTelemetryContext(new Date().getTime());
            //span.requestTelemetry = span.context.getHttpRequestTelemetry();
            requestTelemetry = telemetryClient.StartOperation<RequestTelemetry>("Incoming Message").Telemetry;
            if (parentId != null)
            {
                this.OperationId = parentId.Split(DOT)[0];
                this.Uuid = this.OperationId + DOT + random.Next().ToString("X");
                this.requestTelemetry.Context.Operation.ParentId = parentId;
            }
            else
            {
                this.Uuid = ActivitySpanId.CreateRandom().ToHexString();
                this.OperationId = this.Uuid;
            }
            this.requestTelemetry.Id = PIPE + this.Uuid + DOT;
            this.requestTelemetry.Context.Operation.Id = this.OperationId;
            this.requestTelemetry.Context.Operation.Name = this.Uuid;
            this.OperationName = "MESSAGE " + messageChannelName;
            this.requestTelemetry.Name = this.OperationName;
            this.requestTelemetry.Context.Operation.Name = this.OperationName;
            //TODO: uncomment after 2.5.0 version of Azure App Insights is released
            //requestTelemetry.setAllowAgentToOverrideName(true);
            this.requestTelemetry.Timestamp = timeNow;
        }

        /// <summary>
        /// Create Azure AppInsights span based on received headers for non-specific incoming requests (e.g.gRPC)
        /// </summary>
        /// <param name="headers">Received headers(e.g.HTTP Headers or Message headers)</param>
        /// <returns>Application Insights Span Adapter</returns>
        public void InitForOtherIncomings()
        {
            var headers = this.spanContext.Headers;
            var timeNow = DateTimeOffset.UtcNow;
            logger.LogDebug(MESSAGE_CREATE_SPAN + headers);
            
            string parentId = FetchParentId();

            //span.context = new RequestTelemetryContext(new Date().getTime());
            //span.requestTelemetry = span.context.getHttpRequestTelemetry();
            requestTelemetry = telemetryClient.StartOperation<RequestTelemetry>("Incoming Other").Telemetry;
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                this.requestTelemetry.Context.Operation.ParentId = parentId;
                this.OperationId = parentId.Split(DOT)[0];
                this.Uuid = this.OperationId + DOT + random.Next().ToString("X");
            }
            else
            {
                this.Uuid = ActivitySpanId.CreateRandom().ToHexString();
                this.OperationId = this.Uuid;
            }
            this.requestTelemetry.Id = PIPE + this.Uuid + DOT;
            this.requestTelemetry.Context.Operation.Id = this.OperationId;
            this.requestTelemetry.Context.Operation.Name = this.Uuid;
            //TODO: uncomment after 2.5.0 version of Azure App Insights is released
            //requestTelemetry.setAllowAgentToOverrideName(true);
            this.requestTelemetry.Timestamp = timeNow;
        }

        /// <summary>
        /// Create Azure AppInsights child span for provided parent span
        /// </summary>
        /// <param name="parent">The parent span to link</param>
        /// <returns>Application Insights Span Adapter</returns>
        public void InitForOutgoingRest(ApplicationInsightsSpan parent)
        {
            var timeNow = DateTimeOffset.UtcNow;
            logger.LogDebug(MESSAGE_CREATE_SPAN + "external rest");

            this.remoteDependencyTelemetry = telemetryClient.StartOperation<DependencyTelemetry>("Outgoing REST").Telemetry;
            string parentId = parent.Uuid;
            this.OperationId = parent.OperationId;
            this.Uuid = this.OperationId + DOT + random.Next().ToString("X");
            this.remoteDependencyTelemetry.Name = parent.OperationName;
            this.remoteDependencyTelemetry.Id = PIPE + this.Uuid + DOT;
            this.remoteDependencyTelemetry.Timestamp = timeNow;
            this.remoteDependencyTelemetry.Type = "Http (tracked component)";
            this.remoteDependencyTelemetry.Context.Operation.Id = this.OperationId;
            this.remoteDependencyTelemetry.Context.Operation.ParentId = parentId;
        }

        /// <summary>
        /// Create Azure AppInsights span for outgoing message
        /// </summary>
        /// <param name="tracerAdapter">Reference to the current Tracer adapter for Azure App Insights</param>
        /// <param name="parent">The parent span to link</param>
        /// <param name="messageChannelName">The message channel name we use to publish messsage</param>
        /// <returns>Application Insights Span Adapter</returns>
        public void InitForOutgoingMessage(ApplicationInsightsSpan parent, string messageChannelName)
        {
            var timeNow = DateTimeOffset.UtcNow;
            logger.LogDebug(MESSAGE_CREATE_SPAN + messageChannelName);

            this.remoteDependencyTelemetry = telemetryClient.StartOperation<DependencyTelemetry>("Outgoing Message").Telemetry;
            string parentId = parent.Uuid;
            this.OperationId = parent.OperationId;
            this.Uuid = this.OperationId + DOT + random.Next().ToString("X");
            this.remoteDependencyTelemetry.Id = PIPE + this.Uuid + DOT;
            this.remoteDependencyTelemetry.Name = "PUBLISH " + messageChannelName;
            this.remoteDependencyTelemetry.Timestamp = timeNow;
            this.remoteDependencyTelemetry.Type = "MESSAGE";
            this.remoteDependencyTelemetry.Properties.Add("channel", messageChannelName);
            this.remoteDependencyTelemetry.Context.Operation.Id = this.OperationId;
            this.remoteDependencyTelemetry.Context.Operation.ParentId = parentId;
            this.remoteDependencyTelemetryForMessage = true;
        }

        /// <summary>
        /// This method tries to detect App Insights parentId header
        /// </summary>
        /// <param name="headers">MultiValuedMap containing headers</param>
        /// <returns>String value of parentId or null, if not found</returns>
        private string FetchParentId()
        {
            var headers = this.spanContext.Headers;
            string parentId = null;
            if (headers.ContainsKey(SpanConstants.HEADER_TRACE_ID))
            {
                parentId = headers[SpanConstants.HEADER_TRACE_ID].ToString();
            }
            if (string.IsNullOrWhiteSpace(parentId) && headers.ContainsKey(ApplicationInsightsTracingConstants.HTTP_HEADER_TRACEPARENT))
            {
                foreach (string value in headers[ApplicationInsightsTracingConstants.HTTP_HEADER_TRACEPARENT])
                {
                    if (value.Contains("00-"))
                    {
                        parentId = value.SubstringBetween("00-", "-00").Replace("-", DOT);
                        break;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(parentId) && headers.ContainsKey(ApplicationInsightsTracingConstants.HTTP_HEADER_REQUESTID))
            {
                foreach (string value in headers[ApplicationInsightsTracingConstants.HTTP_HEADER_REQUESTID])
                {
                    if (value.Contains(DOT))
                    {
                        parentId = value;
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(parentId) && headers.ContainsKey(ApplicationInsightsTracingConstants.MESSAGE_HEADER_AZURE_TRACEPARENT))
            {
                foreach (string value in headers[ApplicationInsightsTracingConstants.MESSAGE_HEADER_AZURE_TRACEPARENT])
                {
                    if (!string.IsNullOrWhiteSpace(value) && value.Contains(DOT))
                    {
                        parentId = value;
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(parentId))
                parentId = this.spanContext.TraceId;
            else
                this.spanContext.TraceId = parentId;

            return parentId;
        }

        public ISpan SetName(string name)
        {
            if (requestTelemetry != null)
            {
                OperationName = name;
                requestTelemetry.Name = OperationName;
                requestTelemetry.Context.Operation.Name = OperationName;
            }
            else if (remoteDependencyTelemetry != null)
            {
                remoteDependencyTelemetry.Name = name;
            }
            return this;
        }

        /// <summary>
        /// Set response code tag in the current telemetry object
        /// </summary>
        /// <param name="value">Provided tag value</param>
        /// <returns>This span adapter</returns>
        private ISpan SetResponseCodeTag(string value)
        {
            if (requestTelemetry != null)
            {
                requestTelemetry.ResponseCode = value;
            }
            else if (remoteDependencyTelemetry != null)
            {
                remoteDependencyTelemetry.ResultCode = value;
            }
            return this;
        }

        /// <summary>
        /// Set response success tag in the current telemetry object
        /// </summary>
        /// <param name="value">Provided tag value</param>
        /// <returns>This span adapter</returns>
        private ISpan SetResponseSuccessTag(string value)
        {
            if (requestTelemetry != null)
            {
                requestTelemetry.Success = bool.Parse(value);
            }
            else if (remoteDependencyTelemetry != null)
            {
                remoteDependencyTelemetry.Success = bool.Parse(value);
            }
            return this;
        }

        /// <summary>
        /// Set request target host tag in the current telemetry object
        /// </summary>
        /// <param name="value">Provided tag value</param>
        /// <returns>This span adapter</returns>
        private ISpan SetRequestTargetHostTag(string value)
        {
            tagsRemoteTargetHost = value;
            if (remoteDependencyTelemetry != null && tagsRemoteTargetPort != null)
            {
                remoteDependencyTelemetry.Target = tagsRemoteTargetHost + ":" + tagsRemoteTargetPort + " | " + tagsRemoteTargetHost;
            }
            return this;
        }

        /// <summary>
        /// Set request target port tag in the current telemetry object
        /// </summary>
        /// <param name="value">Provided tag value</param>
        /// <returns>This span adapter</returns>
        private ISpan SetRequestTargetPortTag(string value)
        {
            tagsRemoteTargetPort = value;
            if (remoteDependencyTelemetry != null && tagsRemoteTargetHost != null)
            {
                remoteDependencyTelemetry.Target = tagsRemoteTargetHost + ":" + tagsRemoteTargetPort + " | " + tagsRemoteTargetHost;
            }
            return this;
        }

        /// <summary>
        /// Set request method tag in the current telemetry object
        /// </summary>
        /// <param name="value">Provided tag value</param>
        /// <returns>This span adapter</returns>
        private ISpan setRequestMethodTag(string value)
        {
            if (remoteDependencyTelemetry != null)
            {
                remoteDependencyTelemetry.Properties.Add("Method", value);
            }
            return this;
        }

        /// <summary>
        /// Set request URL tag in the current telemetry object
        /// </summary>
        /// <param name="value">Provided tag value</param>
        /// <returns>This span adapter</returns>
        private ISpan setRequestUrlTag(string value)
        {
            if (requestTelemetry != null)
            {
                try
                {
                    requestTelemetry.Url = new Uri(value);
                }
                catch (Exception e)
                {
                    logger.LogWarning("Can't set Request URL tag into span", e);
                }
            }
            if (remoteDependencyTelemetry != null)
            {
                remoteDependencyTelemetry.Data = value;
                remoteDependencyTelemetry.Properties.Add("URI", value);
            }
            return this;
        }

        public ISpan AddTag(string key, string value)
        {
            switch (key)
            {
                case SpanConstants.TAG_RESPONSE_CODE:
                    return SetResponseCodeTag(value);
                case SpanConstants.TAG_RESPONSE_SUCCESS:
                    return SetResponseSuccessTag(value);
                case SpanConstants.TAG_REQUEST_TARGET_HOST:
                    return SetRequestTargetHostTag(value);
                case SpanConstants.TAG_REQUEST_TARGET_PORT:
                    return SetRequestTargetPortTag(value);
                case SpanConstants.TAG_REQUEST_METHOD:
                    if (requestTelemetry != null
                        && requestTelemetryNameComponents.SetMethod(value)
                        && string.IsNullOrWhiteSpace(OperationName))
                    {
                        SetName(requestTelemetryNameComponents.GetName());
                    }
                    return setRequestMethodTag(value);
                case SpanConstants.TAG_REQUEST_PATH:
                    if (requestTelemetry != null
                        && requestTelemetryNameComponents.SetPath(value)
                        && string.IsNullOrWhiteSpace(OperationName))
                    {
                        SetName(requestTelemetryNameComponents.GetName());
                    }
                    return this;
                case SpanConstants.TAG_REQUEST_URL:
                    return setRequestUrlTag(value);
                default:
                    if (requestTelemetry != null)
                    {
                        requestTelemetry.Properties.Add(key, value);
                    }
                    else if (remoteDependencyTelemetry != null)
                    {
                        remoteDependencyTelemetry.Properties.Add(key, value);
                    }
                    return this;
            }
        }

        public ISpan Start()
        {
            startTime = DateTimeOffset.UtcNow;
            return this;
        }

        public void Finish()
        {
            if (!finished.GetAndSet(true) && startTime != null)
            {
                TimeSpan duration = DateTimeOffset.UtcNow - startTime;//new Duration(java.time.Duration.between(startTime, Instant.now()).toMillis());
                if (requestTelemetry != null)
                {
                    requestTelemetry.Duration = duration;
                    SendTelemetry(requestTelemetry);
                }
                else if (remoteDependencyTelemetry != null)
                {
                    remoteDependencyTelemetry.Duration = duration;
                    SendTelemetry(remoteDependencyTelemetry);
                }
            }
        }

        public ISpan AddError(Exception ex)
        {
            if (requestTelemetry != null)
            {
                requestTelemetry.ResponseCode = "500";
                requestTelemetry.Success = false;
            }
            else if (remoteDependencyTelemetry != null)
            {
                remoteDependencyTelemetry.ResultCode = "500";
                remoteDependencyTelemetry.Success = false;
            }
            ExceptionTelemetry exceptionTelemetry = new ExceptionTelemetry(ex);
            exceptionTelemetry.Timestamp = DateTimeOffset.UtcNow;
            exceptionTelemetry.Context.Operation.Id = OperationId;
            exceptionTelemetry.Context.Operation.Name = OperationName;
            exceptionTelemetry.Context.Operation.ParentId = this.spanContext.TraceId;
            SendTelemetry(exceptionTelemetry);
            return this;
        }

        public ISpan ChildSpan()
        {
            return this.SpanBuilder.ChildSpan(this);
        }

        public ISpan SetInScope()
        {
            // Do Nothing
            this.Tracer.SaveSpanToStack(this);
            return this;
        }

        public void AddPropagationHeaders()
        {
            //if (remoteDependencyTelemetry != null)
            //{
            //    if (remoteDependencyTelemetryForMessage)
            //    {
            //        this.spanContext.Headers.Add(ApplicationInsightsTracingConstants.MESSAGE_HEADER_AZURE_TRACEPARENT, Uuid);
            //    }
            //    else
            //    {
            //        this.spanContext.Headers.Add(ApplicationInsightsTracingConstants.HTTP_HEADER_TRACEPARENT, "00-" + Uuid.Replace('.', '-') + "-00");
            //        this.spanContext.Headers.Add(ApplicationInsightsTracingConstants.HTTP_HEADER_REQUESTID, PIPE + Uuid + DOT);
            //    }
            //}

            if (!this.spanContext.Headers.ContainsKey(SpanConstants.HEADER_TRACE_ID))
                this.spanContext.Headers.Add(SpanConstants.HEADER_TRACE_ID, this.spanContext.TraceId);
        }

        private void SendTelemetry(ITelemetry telemetry)
        {
            tracker.StartNew(() =>
            {
                this.telemetryClient.Track(telemetry);
            });
        }

        public class RequestTelemetryNameComponents
        {
            private string method;
            private string path;

            public bool SetMethod(string method)
            {
                this.method = method;
                return IsComplete();
            }

            public bool SetPath(string path)
            {
                this.path = path;
                return IsComplete();
            }

            public bool IsComplete()
            {
                return method != null && path != null;
            }

            public string GetName()
            {
                return method + " " + path;
            }
        }
    }
}
