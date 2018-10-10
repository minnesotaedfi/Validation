using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{

    /// <summary>
    /// Catches all unhandled exceptions and formats user message to be returned in the HTTP response.
    /// </summary>
    public class ValidationWebExceptionFilterAttribute : ExceptionFilterAttribute
    {
        protected readonly IRequestMessageAccessor _requestAccessor;

        public ValidationWebExceptionFilterAttribute(IRequestMessageAccessor requestAccessor, ILoggingService logger)
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
            // Return them as JSON.
            actionExecutedContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new ObjectContent<object>(new { reason = $"Error processing the request. See log, correlation ID: {corrId}" }, formatter, "application/json")
            };
        }
    }
}