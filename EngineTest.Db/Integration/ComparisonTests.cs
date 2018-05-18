using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Db.Integration
{
    public class ComparisonTests
    {
        [TestClass]
        public class WhenComparingCharacteristicsToConstants : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "require that {Component1}.[StringCharacteristic1] = 'Value' " +
                       "and {Component1}.[BoolCharacteristic1] = true " +
                       "and {Component1}.[DateCharacteristic1] = 1-Jan-2015 " +
                       "and {Component1}.[NumberCharacteristic1] = 100.0 " +
                       "else 'Failed ValueComparisonTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                //match all
                component1.Add(new Component1
                {
                    Id = 1,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                // bad string
                component1.Add(new Component1
                {
                    Id = 2,
                    StringCharacteristic1 = "Bad Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                // bad bool
                component1.Add(new Component1
                {
                    Id = 3,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = false,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                // bad date
                component1.Add(new Component1
                {
                    Id = 4,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 2),
                    NumberCharacteristic1 = 100.0M
                });
                // bad number
                component1.Add(new Component1
                {
                    Id = 5,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.1M
                });
            }

            [TestMethod]
            public void Should_pass_first_data_point()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
            }

            [TestMethod]
            public void Should_contain_errors_not_warnings()
            {
                Assert.IsTrue(RuleErrors.All(x => x.IsError));
            }

            [TestMethod]
            public void Should_have_error_for_each_bad_data()
            {
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 2) != null);
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 3) != null);
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 4) != null);
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 5) != null);
            }

            [TestMethod]
            public void Errors_should_contain_contain_rule_info()
            {
                Assert.IsTrue(RuleErrors.All(x => x.RuleId == "100.0"));
                Assert.IsTrue(RuleErrors.All(x => x.Message == "Failed ValueComparisonTest"));
            }
        }

        [TestClass]
        public class WhenComparingCharacteristics : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "require that {Component1}.[StringCharacteristic1] = {Component2}.[StringCharacteristic2] " +
                       "and {Component1}.[BoolCharacteristic1] = {Component2}.[BoolCharacteristic2] " +
                       "and {Component1}.[DateCharacteristic1] = {Component2}.[DateCharacteristic2] " +
                       "and {Component1}.[NumberCharacteristic1] = {Component2}.[NumberCharacteristic2] " +
                       "else 'Failed CharacteristicComparisonTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                #region Component1
                component1.Add(new Component1
                {
                    Id = 1,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                component1.Add(new Component1
                {
                    Id = 2,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                component1.Add(new Component1
                {
                    Id = 3,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                component1.Add(new Component1
                {
                    Id = 4,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                component1.Add(new Component1
                {
                    Id = 5,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                component1.Add(new Component1
                {
                    Id = 6,
                    StringCharacteristic1 = "Value",
                    BoolCharacteristic1 = true,
                    DateCharacteristic1 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic1 = 100.0M
                });
                #endregion

                #region Component2
                // match
                component2.Add(new Component2
                {
                    Id = 1,
                    StringCharacteristic2 = "Value",
                    BoolCharacteristic2 = true,
                    DateCharacteristic2 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic2 = 100.0M
                });
                // invalid string
                component2.Add(new Component2
                {
                    Id = 2,
                    StringCharacteristic2 = "Bad Value",
                    BoolCharacteristic2 = true,
                    DateCharacteristic2 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic2 = 100.0M
                });
                // invalid bool
                component2.Add(new Component2
                {
                    Id = 3,
                    StringCharacteristic2 = "Value",
                    BoolCharacteristic2 = false,
                    DateCharacteristic2 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic2 = 100.0M
                });
                // invalid date
                component2.Add(new Component2
                {
                    Id = 4,
                    StringCharacteristic2 = "Value",
                    BoolCharacteristic2 = true,
                    DateCharacteristic2 = new System.DateTime(2015, 1, 2),
                    NumberCharacteristic2 = 100.0M
                });
                // invalid number
                component2.Add(new Component2
                {
                    Id = 5,
                    StringCharacteristic2 = "Value",
                    BoolCharacteristic2 = true,
                    DateCharacteristic2 = new System.DateTime(2015, 1, 1),
                    NumberCharacteristic2 = 100.1M
                });
                // missing Id 6
                #endregion
            }

            [TestMethod]
            public void Should_pass_first_data_point()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
            }

            [TestMethod]
            public void Should_contain_errors_not_warnings()
            {
                Assert.IsTrue(RuleErrors.All(x => x.IsError));
            }

            [TestMethod]
            public void Should_have_error_for_each_bad_data()
            {
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 2) != null);
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 3) != null);
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 4) != null);
                Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 5) != null);
                //Assert.IsTrue(RuleErrors.SingleOrDefault(x => x.Id == 6) != null);
            }

            [TestMethod]
            public void Errors_should_contain_contain_rule_info()
            {
                Assert.IsTrue(RuleErrors.All(x => x.RuleId == "100.0"));
                Assert.IsTrue(RuleErrors.All(x => x.Message == "Failed CharacteristicComparisonTest"));
            }
        }

        [TestClass]
        public class WhenComparingToConsts : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "require that {Component1}.[StringCharacteristic1] is in ['one', 'two', 'three'] " +
                       "and {Component1}.[NumberCharacteristic1] is in [1, 2, 3] " +
                       "else 'Failed ConstsComparisonTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "one", NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "two", NumberCharacteristic1 = 2 });
                component1.Add(new Component1 { Id = 3, StringCharacteristic1 = "three", NumberCharacteristic1 = 3 });
                component1.Add(new Component1 { Id = 4, StringCharacteristic1 = "four", NumberCharacteristic1 = 4 });
            }

            [TestMethod]
            public void Should_not_error_on_string_in_constants()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 2));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 3));
            }

            [TestMethod]
            public void Should_error_on_string_not_in_constants()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.Id == 4));
                Assert.IsFalse(RuleErrors.SingleOrDefault(x => x.RuleId == "100.0") == null);
            }
        }

        [TestClass]
        public class WhenComparingToTuples : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "require that {Component1}.[StringCharacteristic1, NumberCharacteristic1] " +
                       "is in [('one',1),('two',2),('three',3)] " +
                       "else 'failed TupleComparisonTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "one", NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "two", NumberCharacteristic1 = 2 });
                component1.Add(new Component1 { Id = 3, StringCharacteristic1 = "three", NumberCharacteristic1 = 3 });
                component1.Add(new Component1 { Id = 4, StringCharacteristic1 = "four", NumberCharacteristic1 = 4 });
            }

            [TestMethod]
            public void Should_not_error_on_string_in_tuple()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 2));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 3));
            }

            [TestMethod]
            public void Should_error_on_string_not_in_tuple()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.Id == 4));
                Assert.IsFalse(RuleErrors.SingleOrDefault(x => x.RuleId == "100.0") == null);
            }
        }

        [TestClass]
        public class WhenComparingToLookups : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "require that {Component1}.[StringCharacteristic1, NumberCharacteristic1] " +
                       "is in {Component2}.[StringCharacteristic2, NumberCharacteristic2] " +
                       "else 'Failed LookupComparisonTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "one", NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "two", NumberCharacteristic1 = 2 });
                component1.Add(new Component1 { Id = 3, StringCharacteristic1 = "three", NumberCharacteristic1 = 3 });
                component1.Add(new Component1 { Id = 4, StringCharacteristic1 = "four", NumberCharacteristic1 = 4 });

                component2.Add(new Component2 { Id = 1, StringCharacteristic2 = "one", NumberCharacteristic2 = 1 });
                component2.Add(new Component2 { Id = 2, StringCharacteristic2 = "two", NumberCharacteristic2 = 2 });
                component2.Add(new Component2 { Id = 3, StringCharacteristic2 = "three", NumberCharacteristic2 = 3 });
            }

            [TestMethod]
            public void Should_not_error_on_string_in_lookup()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 2));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 3));
            }

            [TestMethod]
            public void Should_error_on_string_not_in_lookup()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.Id == 4));
                Assert.IsFalse(RuleErrors.SingleOrDefault(x => x.RuleId == "100.0") == null);
            }
        }

        [TestClass]
        public class WhenComparingUniqueness : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "require that {Component1}.[StringCharacteristic1, NumberCharacteristic1] is unique " +
                       "else 'Failed UniquenessTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, StringCharacteristic1 = "one", NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, StringCharacteristic1 = "two", NumberCharacteristic1 = 2 });
                component1.Add(new Component1 { Id = 3, StringCharacteristic1 = "three", NumberCharacteristic1 = 3 });
                component1.Add(new Component1 { Id = 4, StringCharacteristic1 = "four", NumberCharacteristic1 = 4 });
                component1.Add(new Component1 { Id = 5, StringCharacteristic1 = "four", NumberCharacteristic1 = 4 });
            }

            [TestMethod]
            public void Should_not_error_on_string_in_lookup()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 2));
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 3));
            }

            [TestMethod]
            public void Should_error_on_string_not_in_lookup()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.Id == 4));
                Assert.AreEqual(1, RuleErrors.Count(x => x.Id == 5));
                Assert.AreEqual(2, RuleErrors.Count(x => x.RuleId == "100.0"));
            }
        }

        [TestClass]
        public class WhenComparingExists : BaseTestClass
        {
            protected override string SetRules()
            {
                return "ruleset ValueComparisonTest " +
                       "rule 100.0 " +
                       "when {Component1} exists then " +
                       "require that {Component2} exists " +
                       "else 'Failed ConstsComparisonTest'";
            }

            protected override void SetTestData(List<Component1> component1, List<Component2> component2)
            {
                component1.Add(new Component1 { Id = 1, NumberCharacteristic1 = 1 });
                component1.Add(new Component1 { Id = 2, NumberCharacteristic1 = 2 });

                component2.Add(new Component2 { Id = 1, NumberCharacteristic2 = 1 });
            }

            [TestMethod]
            public void Should_not_error_on_existing_value()
            {
                Assert.IsFalse(RuleErrors.Any(x => x.Id == 1));
            }

            [TestMethod]
            public void Should_error_on_nonexisting_value()
            {
                Assert.AreEqual(1, RuleErrors.Count(x => x.Id == 2));
                Assert.AreEqual(1, RuleErrors.Count(x => x.RuleId == "100.0"));
            }
        }

    }
}
