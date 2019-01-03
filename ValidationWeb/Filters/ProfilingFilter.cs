using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class ProfilingFilter : ActionFilterAttribute
    {
        protected readonly ILoggingService _logger;

        public ProfilingFilter(ILoggingService logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _logger.LogInfoMessage("Controller model bound. Controller action starting.");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _logger.LogInfoMessage("RESPONSE COMPLETED.");
        }
    }
}
