using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace MDE.ValidationPortal
{
    /// <summary>
    /// Initiates logging capability by attaching an ID that will by added to all the log messages for this Request - as well as
    /// a start time to use to determine the age of the request at the time of each log message - used for performance monitoring.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        protected const string RequestStartTimeKeyName = "RequestStartTime";
        protected const string LogCorrelationIdKeyName = "LogCorrelationId";
        protected readonly IApplicationLoggerService _logger;

        public LoggingHandler(IApplicationLoggerService logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attach an ID that will by added to all the log messages for this Request. Starts the clock on performance monitoring
        /// included in all log requests. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            #region Add Logging Context to the Request
            var newGuid = Guid.NewGuid();
            // "B" means "Braces" - {00000000-0000-0000-0000-000000000000}
            var logCorrelationId = newGuid.ToString("B");
            request.Properties.Add(LogCorrelationIdKeyName, logCorrelationId);
            _logger.LogInfoMessage("BEGINNING OF REQUEST ------------------");
            #endregion Add Logging Context to the Request

            #region Add Performance Measuring Context to the Request
            request.Properties.Add(RequestStartTimeKeyName, DateTime.UtcNow);
            #endregion Add Performance Measuring Context to the Request

            #region Dispatch the request.
            var response = await base.SendAsync(request, cancellationToken);
            return response;
            #endregion Dispatch the request.
        }
    }
}
