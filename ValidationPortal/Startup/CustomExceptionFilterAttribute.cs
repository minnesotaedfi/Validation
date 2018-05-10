using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Text;

namespace MDE.ValidationPortal
{
    /// <summary>
    /// Catches all unhandled exceptions and formats user message to be returned in the HTTP response.
    /// </summary>
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        protected readonly IRequestMessageAccessor _requestAccessor;
        protected const string LogCorrelationIdKeyName = "LogCorrelationId";

        public CustomExceptionFilterAttribute(IRequestMessageAccessor requestAccessor)
        {
            _requestAccessor = requestAccessor;
        }

        /// <summary>
        /// Called for all unhandled exceptions; formats user message to be returned in the HTTP response.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var formatter = actionExecutedContext.Request.GetConfiguration().Formatters[0];
            var responseContent = GenerateClientErrorMessage(actionExecutedContext.Exception);

            // Return them as JSON.
            actionExecutedContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new ObjectContent<string>(responseContent, formatter, "application/json")
            };
        }

        /// <summary>
        /// Builds and logs human readable, safe-against-hackers error messages.
        /// </summary>
        /// <returns></returns>
        public string GenerateClientErrorMessage(Exception ex)
        {
            var messageBuilder = new System.Text.StringBuilder();
            messageBuilder.AppendLine("An error occurred while processing the request.");
            object logCorrelationIdObject = null;
            string logCorrelationId;

            if (_requestAccessor.CurrentMessage.Properties.TryGetValue(LogCorrelationIdKeyName, out logCorrelationIdObject))
            {
                logCorrelationId = logCorrelationIdObject.ToString();
            }
            else
            {
                logCorrelationId = "Unknown";
            }
            messageBuilder.AppendLine($"Exception Message: {ex.Message}");
            messageBuilder.AppendLine($"An administrator can provide more information about the error by searching the logs for Correlation ID {logCorrelationId}.");
#if DEBUG
            messageBuilder.AppendLine(ex.ChainInnerExceptionMessages());
#endif
            return messageBuilder.ToString();
        }
    }
}
