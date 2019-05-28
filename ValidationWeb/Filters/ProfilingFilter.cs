using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Filters
{
    public class ProfilingFilter : ActionFilterAttribute
    {
        protected readonly ILoggingService Logger;

        public ProfilingFilter(ILoggingService logger)
        {
            Logger = logger;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Logger.LogInfoMessage("Controller model bound. Controller action starting.");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Logger.LogInfoMessage("RESPONSE COMPLETED.");
        }
    }
}
