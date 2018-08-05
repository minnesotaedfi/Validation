using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            return $"[{currentTime.ToLongTimeString()} {currentTime.ToShortDateString()}] ";
        }

    }
}