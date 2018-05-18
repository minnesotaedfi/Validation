using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Db.Integration
{
    public class ParameterTests
    {
        [TestClass]
        public class WhenSettingGlobalParameters : BaseTestClass
        {
            protected override string SetRules()
            {
                return $"define Param1 = 1, Param2 = today " +
                       "ruleset Foo rule 100.1 require that {Component1}.[NumberCharacteristic1] = Param1 else 'error' " +
                       "ruleset Foo rule 100.2 require that {Component1}.[DateCharacteristic1] = Param2 else 'error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, NumberCharacteristic1 = 1, DateCharacteristic1 = DateProvider.Today });
                component1.Add(new Component1 { Id = 2, NumberCharacteristic1 = 2, DateCharacteristic1 = DateProvider.Today.AddDays(1) });
            }

            [TestMethod]
            public void Should_not_match_second_component()
            {
                Assert.AreEqual(2, RuleErrors.Count);
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.2"));
            }
        }
    }
}
