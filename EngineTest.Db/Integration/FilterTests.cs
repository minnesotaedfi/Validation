using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Db.Integration
{
    public class FilterTests
    {
        public class FilterTestBase : BaseTestClass
        {
            protected override string SetRules()
            {
                return $"collection TestCollection1 includes Ruleset1 " +
                       $"ruleset Ruleset1 " +
                       $"rule 100.1 when collection is TestCollection1 then require {{Component1}}.[NumberCharacteristic1] = 1 else 'error' " +
                       $"rule 100.2 when collection is in [TestCollection1, TestCollection2] then require {{Component1}}.[NumberCharacteristic1] = 1 else 'error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, NumberCharacteristic1 = 2 });
            }
        }

        [TestClass]
        public class FilterMatches : FilterTestBase
        {
            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                base.SetTestData(component1, component2);
                CollectionName = "TestCollection1";
            }

            [TestMethod]
            public void Should_not_run_either_rule()
            {
                Assert.AreEqual(2, RuleErrors.Count);
            }
        }

        [TestClass]
        public class FilterDoesNotMatch : FilterTestBase
        {
            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                base.SetTestData(component1, component2);
                CollectionName = "DoesntMatchCollection";
            }

            [TestMethod]
            public void Should_not_run_either_rule()
            {
                Assert.AreEqual(0, RuleErrors.Count);
            }
        }
    }
}
