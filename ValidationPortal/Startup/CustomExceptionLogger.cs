using System.Web.Http.ExceptionHandling;

namespace MDE.ValidationPortal
{
    /// <summary>
    /// Ensures all unhandled exceptions are logged to the application log.
    /// </summary>
    public class ExporterExceptionLogger : ExceptionLogger
    {
        protected readonly CustomExceptionFilterAttribute _exceptionFilterImplementation;
        protected readonly IApplicationLoggerService _logger;
        protected readonly IRequestMessageAccessor _requestAccessor;
        protected const string LogCorrelationIdKeyName = "LogCorrelationId";

        public ExporterExceptionLogger(IApplicationLoggerService logger, IRequestMessageAccessor requestAccessor)
        {
            _logger = logger;
            _requestAccessor = requestAccessor;
            _exceptionFilterImplementation = new CustomExceptionFilterAttribute(requestAccessor);
        }

        /// <summary>
        /// Ensures all unhandled exceptions are logged to the application log.
        /// </summary>
        /// <param name="context"></param>
        public override void Log(ExceptionLoggerContext context)
        {
            var exception = context.Exception;
            _logger.LogErrorMessage($"EXCEPTION: {exception.Message} \r\n {exception.ChainInnerExceptionMessages()}");
        }
    }
}