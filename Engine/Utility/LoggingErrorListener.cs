using System;

using Antlr4.Runtime;

using Engine.Language;

using log4net;

namespace Engine.Utility
{
    public class LoggingErrorListener : IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
    {
        private readonly Func<string> _fileContextFunc;

        public LoggingErrorListener(Func<string> fileContextFunc = null)
        {
            _fileContextFunc = fileContextFunc ?? EmptyString;
        }

        private static string EmptyString()
        {
            return string.Empty;
        }

        private readonly ILog _log = LogManager.GetLogger(typeof(MsdsListener));

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            using (NDC.Push($"{line}:{charPositionInLine}"))
                _log.Error(msg);
        }

        public void SyntaxError(
            IRecognizer recognizer, 
            IToken offendingSymbol, 
            int line, 
            int charPositionInLine, 
            string msg,
            RecognitionException e)
        {
            var file = _fileContextFunc();

            using (NDC.Push($"{line}:{charPositionInLine}"))
            {
                // _log.Error(msg);

                var ruleset = offendingSymbol.InputStream.ToString();
                ruleset = ruleset.Substring(0, ruleset.IndexOf("\r\n", StringComparison.Ordinal));
                _log.Error($"SYNTAX ERROR AT {ruleset} - line {line} column {charPositionInLine}");
            }
        }
    }
}
