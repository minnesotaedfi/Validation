namespace ValidationWeb.Services.Interfaces
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
