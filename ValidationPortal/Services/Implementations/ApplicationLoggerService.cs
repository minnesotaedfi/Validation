using System;
using System.Collections.Generic;
using log4net;

namespace MDE.ValidationPortal
{
    public class ApplicationLoggerService : IApplicationLoggerService
    {
        protected const string RequestStartTimeKeyName = "RequestStartTime";
        protected const string LogCorrelationIdKeyName = "LogCorrelationId";
        protected readonly ILog _logger;
        protected readonly IRequestMessageAccessor _requestAccessor;
        public ApplicationLoggerService(ILog logger, IRequestMessageAccessor requestAccessor)
        {
            _logger = logger;
            _requestAccessor = requestAccessor;
        }
        public void LogDebugMessage(string message)
        {
            _logger.Debug($"{GetWebLogMessageHeader()} {message}");
        }
        public void LogInfoMessage(string message)
        {
            _logger.Info($"{GetWebLogMessageHeader()} {message}");
        }
        public void LogWarningMessage(string message)
        {
            _logger.Warn($"{GetWebLogMessageHeader()} {message}");
        }
        public void LogErrorMessage(string message)
        {
            _logger.Error($"{GetWebLogMessageHeader()} {message}");
        }
        public void LogFatalMessage(string message)
        {
            _logger.Fatal($"{GetWebLogMessageHeader()} {message}");
        }

        public string GetLogCorrelationId()
        {
            object logCorrelationIdObject;
            string logCorrelationId;
            var hasMessage = _requestAccessor?.CurrentMessage?.Properties != null;
            if (hasMessage && _requestAccessor.CurrentMessage.Properties.TryGetValue(LogCorrelationIdKeyName, out logCorrelationIdObject))
            {
                logCorrelationId = logCorrelationIdObject.ToString();
            }
            else
            {
                logCorrelationId = "{Request ID Missing}";
            }
            return logCorrelationId;
        }

        public DateTime GetRequestStartTime()
        {
            object requestStartTimeObject;
            DateTime requestStartTime;
            if (_requestAccessor.CurrentMessage.Properties.TryGetValue(RequestStartTimeKeyName, out requestStartTimeObject))
            {
                requestStartTime = (DateTime)requestStartTimeObject;
            }
            else
            {
                requestStartTime = DateTime.UtcNow;
            }
            return requestStartTime;
        }

        private string GetWebLogMessageHeader()
        {
            DateTime now = DateTime.UtcNow;
            DateTime requestStartTime = GetRequestStartTime();
            string logCorrelationId = GetLogCorrelationId();
            var elapsed = Convert.ToInt64((now - requestStartTime).TotalMilliseconds).ToString();
            return $"{logCorrelationId} [{elapsed} msec]: ";
        }
    }
}