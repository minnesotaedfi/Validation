using System;
using System.Web;
using log4net;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class LoggingService : ILoggingService
    {
        protected readonly ILog Logger;

        public LoggingService(ILog logger)
        {
            Logger = logger;
        }

        public void LogDebugMessage(string message)
        {
            Logger.Debug($"{GetLogMessageHeader()} {message}");
        }

        public void LogInfoMessage(string message)
        {
            Logger.Info($"{GetLogMessageHeader()} {message}");
        }

        public void LogWarningMessage(string message)
        {
            Logger.Warn($"{GetLogMessageHeader()} {message}");
        }

        public void LogErrorMessage(string message)
        {
            Logger.Error($"{GetLogMessageHeader()} {message}");
        }

        public void LogFatalMessage(string message)
        {
            Logger.Fatal($"{GetLogMessageHeader()} {message}");
        }

        private string GetLogMessageHeader()
        {
            var currentTime = DateTime.UtcNow;
            var corrId = string.Empty;

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains("CorrelationID"))
                {
                    corrId = HttpContext.Current.Items["CorrelationID"].ToString();
                }
                else
                {
                    corrId = $"[Request {Guid.NewGuid():N}] ";
                    HttpContext.Current.Items.Add("CorrelationID", corrId);
                }
            }
            return $"[{currentTime.ToLongTimeString()} {corrId}{currentTime.ToShortDateString()}] ";
        }

    }
}