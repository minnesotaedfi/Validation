using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace ValidationWeb.Services
{
    public class LoggingService : ILoggingService
    {
        protected readonly ILog _logger;

        public LoggingService(ILog logger)
        {
            _logger = logger;
        }
        public void LogDebugMessage(string message)
        {
            _logger.Debug($"{GetLogMessageHeader()} {message}");
        }
        public void LogInfoMessage(string message)
        {
            _logger.Info($"{GetLogMessageHeader()} {message}");
        }
        public void LogWarningMessage(string message)
        {
            _logger.Warn($"{GetLogMessageHeader()} {message}");
        }
        public void LogErrorMessage(string message)
        {
            _logger.Error($"{GetLogMessageHeader()} {message}");
        }
        public void LogFatalMessage(string message)
        {
            _logger.Fatal($"{GetLogMessageHeader()} {message}");
        }

        private string GetLogMessageHeader()
        {
            DateTime now = DateTime.UtcNow;
            DateTime currentTime = DateTime.UtcNow;
            string corrId = string.Empty;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains("CorrelationID"))
                {
                    corrId = HttpContext.Current.Items["CorrelationID"].ToString();
                }
                else
                {
                    corrId = $"[Request {Guid.NewGuid().ToString("N")}] ";
                    HttpContext.Current.Items.Add("CorrelationID", corrId);
                }
            }
            return $"[{currentTime.ToLongTimeString()} {corrId}{currentTime.ToShortDateString()}] ";
        }

    }
}