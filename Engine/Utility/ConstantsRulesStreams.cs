using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Utility
{
    public class ConstantsRulesStreams : IRulesStreamsProvider
    {
        private readonly IEnumerable<IConstantValueProvider> _constantValueProviders;

        public ConstantsRulesStreams(IEnumerable<IConstantValueProvider> constantValueProviders)
        {
            _constantValueProviders = constantValueProviders;
        }

        public IEnumerable<string> RulesText => _constantValueProviders
            .SelectMany(x => x.Values.Select(y => $"define {y.Key} = {y.Value}"));

        Stream[] IRulesStreamsProvider.Streams
        {
            get
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                {
                    foreach (var s in RulesText)
                    {
                        writer.WriteLine(s);
                    }
                    writer.Flush();
                }
                stream.Position = 0;
                return new Stream[] { stream };
            }
        }
    }
}