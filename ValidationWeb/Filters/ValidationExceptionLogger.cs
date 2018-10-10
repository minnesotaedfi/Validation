using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class ValidationExceptionLogger : ExceptionLogger
    {
        protected readonly ILoggingService _logger;
        protected readonly IRequestMessageAccessor _requestAccessor;

        public ValidationExceptionLogger(ILoggingService logger, IRequestMessageAccessor requestAccessor)
        {
            _logger = logger;
            _requestAccessor = requestAccessor;
        }

        /// <summary>
        /// Ensures all unhandled exceptions are logged to the application log.
        /// </summary>
        /// <param name="context"></param>
        public override void Log(ExceptionLoggerContext context)
        {
            string corrId = "(none)";
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

            _logger.LogErrorMessage($"Error processing the request.See log, correlation ID: {corrId} \r\n {context.Exception.ChainInnerExceptionMessages()}");
        }
    }
}