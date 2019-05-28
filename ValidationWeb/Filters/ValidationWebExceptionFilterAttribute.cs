using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Filters
{
    /// <summary>
    /// Catches all unhandled exceptions and formats user message to be returned in the HTTP response.
    /// </summary>
    public class ValidationWebExceptionFilterAttribute : ExceptionFilterAttribute
    {
        protected readonly IRequestMessageAccessor RequestAccessor;

        public ValidationWebExceptionFilterAttribute(IRequestMessageAccessor requestAccessor)
        {
            RequestAccessor = requestAccessor;
        }

        /// <summary>
        /// Called for all unhandled exceptions; formats user message to be returned in the HTTP response.
        /// </summary>
        /// <param name="actionExecutedContext">Action Context</param>
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
                    corrId = $"[Request {Guid.NewGuid():N}] ";
                    HttpContext.Current.Items.Add("CorrelationID", corrId);
                }
            }

            // Return them as JSON.
            actionExecutedContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new ObjectContent<object>(
                    new
                    {
                        reason = $"Error processing the request. See log, correlation ID: {corrId}"
                    }, 
                    formatter, 
                    "application/json")
            };
        }
    }
}