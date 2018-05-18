using System.Linq;
using System.Text.RegularExpressions;
using Engine.Models;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Language
{
    public class ConditionTests
    {
        [TestClass]
        public class WhenCompoundCondition
        {
            private Model _model;

            private string _text =
                "ruleset A rule 1.1 require (1<2 or 2<=3 or 3=3) or (3=>2 or 2>1 or true<>false) else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_evaluate_all_conditions()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains("1 < 2"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("2 <= 3"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("3 = 3"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("3 >= 2"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("2 > 1"));
            }

            [TestMethod]
            public void Should_evaluate_all_operations()
            {
                Assert.AreEqual(5, Regex.Matches(_model.Rules[0].Sql, " OR ").Count);
            }
        }

        [TestClass]
        public class WhenPatternCondition
        {
            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A}.[B] matches '[0-9][0-9]' else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_like_statement()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains("LIKE '[0-9][0-9]'"));
            }
        }

        [TestClass]
        public class WhenInConsts
        {
            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A}.[B] is in [1,2,3] else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_in_statement()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains("IN (1, 2, 3)"));
            }
        }

        [TestClass]
        public class WhenInTuples
        {
            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A}.[B,C] is in [(1,2),(3,4)] else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_exists_statement()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains("EXISTS"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("(1, 2), (3, 4)"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("= [A].[B]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("= [A].[C]"));
            }
        }

        [TestClass]
        public class WhenInLookups
        {
            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A}.[B,C] is in {L}.[F,G] else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_exists_statement()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains("EXISTS"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("(SELECT [F], [G] FROM [dbo].[L])"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("= [A].[B]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("= [A].[C]"));
            }

            [TestMethod]
            public void Should_include_component_in_components()
            {
                Assert.AreEqual(2, _model.Components.Count);
                Assert.IsTrue(_model.Components.Any(x => x.ComponentName == "A"));
                Assert.IsTrue(_model.Components.Any(x => x.CharacteristicName == "B"));
                Assert.IsTrue(_model.Components.Any(x => x.CharacteristicName == "C"));
            }

            [TestMethod]
            public void Should_not_include_lookup_table_in_components()
            {
                Assert.AreEqual(2, _model.Components.Count);
                Assert.IsFalse(_model.Components.Any(x => x.ComponentName == "L"));
                Assert.IsFalse(_model.Components.Any(x => x.CharacteristicName == "F"));
                Assert.IsFalse(_model.Components.Any(x => x.CharacteristicName == "G"));
            }
        }

        [TestClass]
        public class WhenExists1
        {
            //  SELECT [Ids].[Id], 1.1 [RuleId], 'true' [IsError], 'error' [Message]
            //  FROM(SELECT [Id] FROM [A] UNION SELECT [Id] FROM[B]) [Ids]
            //  LEFT OUTER JOIN [A] ON [Ids].[Id] = [A].[Id]
            //  LEFT OUTER JOIN [B] ON [Ids].[Id] = [B].[Id]
            //  WHERE NOT([A].[B] IS NULL AND [C].[D] IS NOT NULL)

            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A}.[B] exists and {C}.[D] does not exist else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_condition_statement()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains("[A].[B] IS NOT NULL"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("[C].[D] IS NULL"));
            }
        }

        [TestClass]
        public class WhenExists2
        {
            //  SELECT [Ids].[Id], 1.1 [RuleId], 'true' [IsError], 'error' [Message]
            //  FROM(SELECT [Id] FROM [A] UNION SELECT [Id] FROM[B]) [Ids]
            //  LEFT OUTER JOIN [A] ON [Ids].[Id] = [A].[Id]
            //  LEFT OUTER JOIN [B] ON [Ids].[Id] = [B].[Id]
            //  WHERE NOT([A].[Id] IS NULL AND [B].[Id] IS NOT NULL)

            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A} exists and {B} does not exist else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_condition_statement()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains("[A].[Id] IS NOT NULL"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("[B].[Id] IS NULL"));
            }
        }

        [TestClass]
        public class WhenIsUnique
        {
            private Model _model;
            private string _text = "ruleset A rule 1.1 require {A}.[B, C] is unique else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_select_count_statement()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains("SELECT [B], [C] FROM [dbo].[A]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("[T0].[B] = [A].[B]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("[T0].[C] = [A].[C]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("GROUP BY [B], [C]"));
            }
        }
    }
}
