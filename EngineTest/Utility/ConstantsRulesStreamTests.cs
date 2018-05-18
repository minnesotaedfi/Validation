using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Utility
{
    public class ConstantsRulesStreamTests
    {
        private class TestValueProvider : IConstantValueProvider
        {
            public IDictionary<string, string> Values { get; }

            public TestValueProvider(params KeyValuePair<string, string>[] values)
            {
                Values = values.ToDictionary(x => x.Key, x => x.Value);
            }
        }

        [TestClass]
        public class WhenCreatingRulesStream
        {
            private ConstantsRulesStreams _constants;

            [TestInitialize]
            public void Setup()
            {
                _constants = new ConstantsRulesStreams(new[]
                {
                    new TestValueProvider(
                        new KeyValuePair<string, string>("key1", "value1"),
                        new KeyValuePair<string, string>("key2", "value2")),
                    new TestValueProvider(
                        new KeyValuePair<string, string>("key3", "value3"),
                        new KeyValuePair<string, string>("key1", "value1")),
                });
            }

            [TestMethod]
            public void Should_create_rows_for_all_values()
            {
                Assert.IsTrue(_constants.RulesText.Contains("define key1 = value1"));
                Assert.IsTrue(_constants.RulesText.Contains("define key2 = value2"));
                Assert.IsTrue(_constants.RulesText.Contains("define key3 = value3"));
                Assert.AreEqual(4, _constants.RulesText.Count());
            }

            [TestMethod]
            public void Should_preserve_order_of_values()
            {
                var allrules = string.Join("\r", _constants.RulesText);
                Console.WriteLine(allrules);
                Assert.IsTrue(allrules.IndexOf("key2", StringComparison.Ordinal) < allrules.IndexOf("key3", StringComparison.Ordinal));
            }
        }
    }
}
