using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runner;

namespace EngineTest.Utility
{
    public class SettingsConstantValueProviderTests
    {
        [TestClass]
        public class WhenReadingSettingsConstants
        {
            private SettingsConstantValueProvider _provider;

            [TestInitialize]
            public void Setup()
            {
                _provider = new SettingsConstantValueProvider();
            }

            [TestMethod]
            public void Should_read_from_configuration_file()
            {
                Assert.IsTrue(_provider.Values.ContainsKey("TestName"));
                Assert.AreEqual("testValue", _provider.Values["TestName"]);
                Assert.IsFalse(_provider.Values.ContainsKey("IgnoredName"));
                Assert.IsFalse(_provider.Values.ContainsKey("Name"));
            }
        }
    }
}