using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface ILoggingService
    {
        void LogDebugMessage(string message);
        void LogInfoMessage(string message);
        void LogWarningMessage(string message);
        void LogErrorMessage(string message);
        void LogFatalMessage(string message);
    }
}
