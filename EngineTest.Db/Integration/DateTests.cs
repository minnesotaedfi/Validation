using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Db.Integration
{
    public class DateTests
    {
        [TestClass]
        public class WhenUsingTodayKeyword : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset R100 rule 100.1 require that {Component1}.[DateCharacteristic1] = today else 'rule 100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, DateCharacteristic1 = DateProvider.Today });
                component1.Add(new Component1 { Id = 2, DateCharacteristic1 = DateProvider.Today.AddDays(1) });
            }

            [TestMethod]
            public void Should_have_one_failure()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_error_on_second_element()
            {
                Assert.IsTrue(RuleErrors.All(x => x.Id == 2));
            }
        }

        [TestClass]
        public class WhenUsingCardinalDate : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset R100 rule 100.1 require that {Component1}.[DateCharacteristic1] = first Saturday in July else 'rule 100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, DateCharacteristic1 = DateProvider.Today });
                component1.Add(new Component1 { Id = 2, DateCharacteristic1 = DateProvider.Today.AddDays(1) });
            }

            [TestMethod]
            public void Should_have_one_failure()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_error_on_second_element()
            {
                Assert.IsTrue(RuleErrors.All(x => x.Id == 2));
            }
        }

        [TestClass]
        public class WhenUsingDifferentialDate : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset R100 rule 100.1 require that {Component1}.[DateCharacteristic1] = 1 month before 1-August else 'rule 100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, DateCharacteristic1 = DateProvider.Today });
                component1.Add(new Component1 { Id = 2, DateCharacteristic1 = DateProvider.Today.AddDays(1) });
            }

            [TestMethod]
            public void Should_have_one_failure()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_error_on_second_element()
            {
                Assert.IsTrue(RuleErrors.All(x => x.Id == 2));
            }
        }

        [TestClass]
        public class WhenUsingIntrinsicTimePeriod : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset R100 rule 100.1 require that years since {Component1}.[DateCharacteristic1] >= 5 else 'rule 100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, DateCharacteristic1 = DateProvider.Today.AddYears(-5) });
                component1.Add(new Component1 { Id = 2, DateCharacteristic1 = DateProvider.Today.AddDays(1).AddYears(-5) });
            }

            [TestMethod]
            public void Should_have_one_failure()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_error_on_second_element()
            {
                Assert.IsTrue(RuleErrors.All(x => x.Id == 2));
            }
        }

        [TestClass]
        public class WhenUsingDateOperation : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset R100 " +
                       "rule 100.1 require the earliest of ({Component1}.[DateCharacteristic1], 1-Jul) = {Component1}.[DateCharacteristic2] else 'rule 100.1 error'" +
                       "rule 100.2 require that the latest in ({Component1}.[DateCharacteristic1], 1-Jul) = {Component1}.[DateCharacteristic2] else 'rule 100.2 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, DateCharacteristic1 = DateProvider.Today.AddDays(0), DateCharacteristic2 = DateProvider.Today.AddDays(0) });
                component1.Add(new Component1 { Id = 2, DateCharacteristic1 = DateProvider.Today.AddDays(1), DateCharacteristic2 = DateProvider.Today.AddDays(0) });
                component1.Add(new Component1 { Id = 3, DateCharacteristic1 = DateProvider.Today.AddDays(2), DateCharacteristic2 = DateProvider.Today.AddDays(2) });
            }

            [TestMethod]
            public void Should_compute_earliest_dates()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.RuleId == "100.1"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 3));
            }

            [TestMethod]
            public void Should_compute_latest_dates()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.RuleId == "100.2"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2));
            }
        }

        [TestClass]
        public class WhenComputingTimeSinceWithComponents : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset R100 rule 100.1 " +
                       "require that days since {Component1}.[DateCharacteristic1] " +
                       "as of {Component1}.[DateCharacteristic2] = {Component1}.[NumberCharacteristic1] " +
                       "else 'rule 100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, DateCharacteristic1 = DateProvider.Today, DateCharacteristic2 = DateProvider.Today.AddDays(1), NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, DateCharacteristic1 = DateProvider.Today, DateCharacteristic2 = DateProvider.Today.AddDays(2), NumberCharacteristic1 = 2 });
                component1.Add(new Component1 { Id = 3, DateCharacteristic1 = DateProvider.Today, DateCharacteristic2 = DateProvider.Today.AddDays(3), NumberCharacteristic1 = 1 });
            }

            [TestMethod]
            public void Should_subtract_days_between_dates()
            {
                Assert.AreEqual(1, RuleErrors.Count);
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 3));
            }
        }
    }
}