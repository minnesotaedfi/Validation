using System;
using System.Text;

namespace ValidationWeb.Utility
{
    public static class ExceptionExtension
    {
        public static string ChainInnerExceptionMessages(this Exception ex)
        {
            var exceptionMessages = new StringBuilder();
            while (ex != null)
            {
                exceptionMessages.AppendLine(ex.Message);
                exceptionMessages.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }

            return exceptionMessages.ToString();
        }
    }
}