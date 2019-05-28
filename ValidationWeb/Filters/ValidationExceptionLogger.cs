using System;
using System.Web;
using System.Web.Http.ExceptionHandling;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Filters
{
    public class ValidationExceptionLogger : ExceptionLogger
    {
        protected readonly ILoggingService Logger;
        protected readonly IRequestMessageAccessor RequestAccessor;

        public ValidationExceptionLogger(ILoggingService logger, IRequestMessageAccessor requestAccessor)
        {
            Logger = logger;
            RequestAccessor = requestAccessor;
        }

        /// <summary>
        /// Ensures all unhandled exceptions are logged to the application log.
        /// </summary>
        /// <param name="context">Exception Logger Context</param>
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
                    corrId = $"[Request {Guid.NewGuid():N}] ";
                    HttpContext.Current.Items.Add("CorrelationID", corrId);
                }
            }

            Logger.LogErrorMessage($"Error processing the request.See log, correlation ID: {corrId} \r\n {context.Exception.ChainInnerExceptionMessages()}");
        }
    }
}