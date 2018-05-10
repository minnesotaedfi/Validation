using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MDE.ValidationPortal
{
    public class ProfilingFilter : ActionFilterAttribute
    {
        protected readonly IApplicationLoggerService _logger;

        public ProfilingFilter(IApplicationLoggerService logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _logger.LogInfoMessage(string.Format("Controller model bound. Controller action starting."));
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _logger.LogInfoMessage(string.Format("RESPONSE COMPLETED."));
        }
    }
}