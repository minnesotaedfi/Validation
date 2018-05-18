using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Db.Integration
{
    public class AggregateTests
    {
        [TestClass]
        public class IntrinsicArithmetic : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicArithmeticTests " +
                       "rule 100.1 " +
                       "require sum ({Component1}.[NumberCharacteristic1, NumberCharacteristic2]) = 10 " +
                       "else '100.1 error' " +
                       "rule 100.2 " +
                       "require max ({Component1}.[NumberCharacteristic1, NumberCharacteristic2]) = 8 " +
                       "else '100.2 error' " +
                       "rule 100.3 " +
                       "require min ({Component1}.[NumberCharacteristic1, NumberCharacteristic2]) = 2 " +
                       "else '100.3 error' ";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, NumberCharacteristic1 = 2, NumberCharacteristic2 = 8 });
                component1.Add(new Component1 { Id = 2, NumberCharacteristic1 = 2, NumberCharacteristic2 = 8 });
                component1.Add(new Component1 { Id = 2, NumberCharacteristic1 = 3, NumberCharacteristic2 = 6 });
                component1.Add(new Component1 { Id = 3, NumberCharacteristic1 = 10 }); // null value test data
                component1.Add(new Component1 { Id = 4, NumberCharacteristic1 = 8 });
                component1.Add(new Component1 { Id = 5, NumberCharacteristic1 = 2 });
            }

            [TestMethod]
            public void Should_return_three_errors()
            {
                Assert.IsTrue(RuleErrors.Count == 9);
            }

            [TestMethod]
            public void Should_show_correct_error_for_sum_rule()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 4 && x.RuleId == "100.1"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 5 && x.RuleId == "100.1"));
            }

            [TestMethod]
            public void Should_show_correct_error_for_max_rule()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.2"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 3 && x.RuleId == "100.2"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 5 && x.RuleId == "100.2"));
            }

            [TestMethod]
            public void Should_show_correct_error_for_min_rule()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.3"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 3 && x.RuleId == "100.3"));
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 4 && x.RuleId == "100.3"));
            }
        }

        [TestClass]
        public class IntrinsicAggregate : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests " +
                       "rule 100.1 require count ({Component1}) = 4 else '100.1 error' \r\n" +
                       "rule 100.2 require count ({Component1} by [StringCharacteristic1]) = 2 else '100.2 error' \r\n" +
                       "rule 100.3 require sum ({Component1}.[NumberCharacteristic1]) = 20 else '100.3 error' \r\n" +
                       "rule 100.4 require sum ({Component1}.[NumberCharacteristic1] by [StringCharacteristic1]) = 10 else '100.4 error' \r\n";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a", NumberCharacteristic1 = 4 });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a", NumberCharacteristic1 = 6 });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "b", NumberCharacteristic1 = 3 });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "b", NumberCharacteristic1 = 7 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "a", NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "a", NumberCharacteristic1 = 2 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "b", NumberCharacteristic1 = 3 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "b", NumberCharacteristic1 = 4 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "c", NumberCharacteristic1 = 5 });
            }

            [TestMethod]
            public void Should_return_four_errors()
            {
                Assert.IsTrue(RuleErrors.Count == 4);
            }

            [TestMethod]
            public void Should_count_across_all()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
            }

            [TestMethod]
            public void Should_count_across_group()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.2"));
            }

            [TestMethod]
            public void Should_sum_across_all()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.3"));
            }

            [TestMethod]
            public void Should_sum_across_group()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.4"));
            }
        }

        [TestClass]
        public class IntrinsicFilteredAggregate1 : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests \r\n" +
                       "rule 100.1 require count ({Component1}) = 2 else '100.1 error' \r\n" +
                       "rule 100.2 require count ({Component1} when {Component1}.[BoolCharacteristic1] = true) = 1 else '100.2 error' \r\n";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, BoolCharacteristic1 = true });
                component1.Add(new Component1 { Id = 1, BoolCharacteristic1 = false });
                component1.Add(new Component1 { Id = 2, BoolCharacteristic1 = false });
            }

            [TestMethod]
            public void Should_return_one_error()
            {
                Assert.IsTrue(RuleErrors.Count == 2);
            }

            [TestMethod]
            public void Should_count_without_when()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
            }

            [TestMethod]
            public void Should_count_with_when()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.2"));
            }

        }

        [TestClass]
        public class IntrinsicFilteredAggregate2 : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests \r\n" +
                       "rule 100.1 require sum ({Component1}.[NumberCharacteristic2] " +
                       "when {Component1}.[NumberCharacteristic1] = {Component2}.[NumberCharacteristic1] and {Component2}.[BoolCharacteristic1] = true " +
                       ") = 14 else '100.1 error' \r\n";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, NumberCharacteristic1 = 1, NumberCharacteristic2 = 3 });
                component1.Add(new Component1 { Id = 1, NumberCharacteristic1 = 2, NumberCharacteristic2 = 5 });

                var date = DateTime.Today;
                component2.Add(new Component2 { Id = 1, NumberCharacteristic1 = 1, DateCharacteristic1 = date.AddDays(0), StringCharacteristic1 = "1", BoolCharacteristic1 = true });
                component2.Add(new Component2 { Id = 1, NumberCharacteristic1 = 2, DateCharacteristic1 = date.AddDays(1), StringCharacteristic1 = "1", BoolCharacteristic1 = false });
                component2.Add(new Component2 { Id = 1, NumberCharacteristic1 = 1, DateCharacteristic1 = date.AddDays(2), StringCharacteristic1 = "1", BoolCharacteristic1 = true });
                component2.Add(new Component2 { Id = 1, NumberCharacteristic1 = 2, DateCharacteristic1 = date.AddDays(3), StringCharacteristic1 = "1", BoolCharacteristic1 = true });
                component2.Add(new Component2 { Id = 1, NumberCharacteristic1 = 1, DateCharacteristic1 = date.AddDays(4), StringCharacteristic1 = "1", BoolCharacteristic1 = true });
            }

            [TestMethod]
            public void Should_return_one_error()
            {
                Assert.IsTrue(RuleErrors.Count == 0);
            }

            [TestMethod]
            public void Should_count_without_when()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.RuleId == "100.1"));
            }
        }

        [TestClass]
        public class IntrinsicFilteredAggregate3 : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests \r\n" +
                       "rule 100.1 " +
                       "when count({Component1} when {Component1}.[StringCharacteristic1] = 'a') > 0 then " +
                       "require count({Component1} when {Component1}.[StringCharacteristic1] = 'b') > 0 " +
                       "else '100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a" });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "b" });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "a" });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "c" });
            }

            [TestMethod]
            public void Should_have_one_error()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_fail_for_second_data_element()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
            }
        }

        [TestClass]
        public class IntrinsicFilteredAggregate4 : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests \r\n" +
                       "rule 100.1 " +
                       "require count({Component1} by [StringCharacteristic1, StringCharacteristic2]) = count({Component1} by [StringCharacteristic1]) " +
                       "else '100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a", StringCharacteristic2 = "b" });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a", StringCharacteristic2 = "b" });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "a", StringCharacteristic2 = "b" });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "a", StringCharacteristic2 = "c" });
            }

            [TestMethod]
            public void Should_have_one_error()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_fail_for_second_data_element()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
            }
        }

        [TestClass]
        public class IntrinsicFilteredAggregate5 : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests \r\n" +
                       "rule 100.1 " +
                       "when {Component1} exists then " +
                       "require count({Component1} when {Component2}.[StringCharacteristic1] = 'a') > 0 " +
                       "else '100.1 error'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1 });
                component1.Add(new Component1 { Id = 2 });

                component2.Add(new Component2 { Id = 1, StringCharacteristic1 = "a" });
                component2.Add(new Component2 { Id = 1, StringCharacteristic1 = "b" });
                component2.Add(new Component2 { Id = 2, StringCharacteristic1 = "b" });
                component2.Add(new Component2 { Id = 3, StringCharacteristic1 = "a" });
            }

            [TestMethod]
            public void Should_have_one_error()
            {
                Assert.AreEqual(1, RuleErrors.Count);
            }

            [TestMethod]
            public void Should_fail_for_second_data_element()
            {
                Assert.IsTrue(RuleErrors.Any(x => x.Id == 2 && x.RuleId == "100.1"));
            }
        }

        [TestClass]
        public class IntrinsicDateArithmetic : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset IntrinsicAggregateTests \r\n" +
                       "rule 100.1 require max ({Component1}.[DateCharacteristic1, DateCharacteristic2]) = 10-Jan-2000 else '100.1 error' \r\n" +
                       "rule 100.2 require min ({Component1}.[DateCharacteristic1, DateCharacteristic2]) = 1-Jan-2000 else '100.1 error' \r\n";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                var date1 = new DateTime(2000, 1, 1);
                var date2 = new DateTime(2000, 1, 10);
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a", DateCharacteristic1 = date1, DateCharacteristic2 = date2 });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "a", DateCharacteristic1 = date2, DateCharacteristic2 = date1 });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "b", DateCharacteristic1 = date2 });
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "b", DateCharacteristic2 = date2 });
            }

            [TestMethod]
            public void Should_compute_max()
            {
                Assert.IsTrue(RuleErrors.Count(x => x.RuleId == "100.1") == 0);
            }

            [TestMethod]
            public void Should_compute_min()
            {
                Assert.IsTrue(RuleErrors.Count(x => x.RuleId == "100.2") == 1);
            }

        }
    }
}
