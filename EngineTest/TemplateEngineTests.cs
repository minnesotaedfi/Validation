using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Templates;

namespace EngineTest
{
    public class TemplateEngineTests
    {
        [TestClass]
        public class WhenUsingTemplateEngine
        {
            private TemplateEngine _engine;

            [TestInitialize]
            public void Setup()
            {
                _engine = new TemplateEngine();
            }

            [TestMethod]
            public void Should_succeed_when_using_known_template()
            {
                var data = new { name = "world" };
                var result = _engine.Generate("Test", data);
                Assert.AreEqual("hello, world\r\n", result);
            }

            [TestMethod]
            public void Should_leave_blanks_when_data_is_not_correct()
            {
                var data = new { };
                var result = _engine.Generate("Test", data);
                Assert.AreEqual("hello, \r\n", result);
            }


            [TestMethod, ExpectedException(typeof(Exception))]
            public void Should_fail_when_using_unknown_template()
            {
                var data = new { name = "world" };
                var result = _engine.Generate("Boom", data);
            }

        }
    }
}
