using System.IO;
using System.Linq;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Utility
{
    public class DirectoryRulesStreamTests
    {
        [TestClass]
        public class WhenCreatingRulesStream
        {
            private FileStream[] _streams;

            [TestInitialize]
            public void Setup()
            {
                _streams = new DirectoryRulesStreams("Utility").Streams.Cast<FileStream>().ToArray();
            }

            [TestMethod]
            public void Should_load_all_rules_files()
            {
                Assert.IsTrue(_streams.Any((FileStream x) => x.Name.Contains("Test1.rules")));
                Assert.IsTrue(_streams.Any((FileStream x) => x.Name.Contains("Test2.rules")));
                Assert.IsFalse(_streams.Any((FileStream x) => x.Name.Contains("Test1.txt")));
            }
        }
    }
}
