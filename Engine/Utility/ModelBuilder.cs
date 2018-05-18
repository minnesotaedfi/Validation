using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Engine.Language;
using Engine.Models;
using log4net;

namespace Engine.Utility
{
    public class ModelBuilder
    {
        private readonly Stream[] _streams;

        public ModelBuilder(params Stream[] streams)
        {
            _streams = streams;
        }

        public Model Build(IDateProvider dateProvider = null, ISchemaProvider schemaProvider = null)
        {
            var model = new Model();
            var walker = new ParseTreeWalker();
            var listener = new MsdsListener(model)
            {
                DateProvider = dateProvider ?? new DateProvider(),
                SchemaProvider = schemaProvider ?? new SchemaProvider()
            };
            foreach (var stream in _streams)
            {
                var filestream = stream as FileStream;
                var filename = filestream == null
                    ? string.Empty
                    : Path.GetFileNameWithoutExtension(((FileStream)stream).Name);
                using (NDC.Push(Path.GetFileNameWithoutExtension(filename)))
                {
                    var lexer = new MsdsLexer(new AntlrInputStream(stream));

                    lexer.RemoveErrorListeners();
                    lexer.AddErrorListener(new LoggingErrorListener());

                    var tokens = new CommonTokenStream(lexer);

                    var parser = new MsdsParser(tokens);
                    parser.RemoveErrorListeners();
                    parser.AddErrorListener(new LoggingErrorListener());

                    walker.Walk(listener, parser.file());
                }
            }
            return model;
        }
    }
}
