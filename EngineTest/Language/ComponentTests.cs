using System.Linq;
using System.Text.RegularExpressions;
using Engine.Models;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Language
{
    public class ComponentTests
    {
        [TestClass]
        public class WhenConstructingUnfilteredRulesWithComponents
        {
            private Model _model;
            private const string Component1 = "Component1";
            private const string Component2 = "Component2";
            private readonly string _text = $@"ruleset Foo rule 100.0 require {{{Component1}}}.[A] = F({{{Component2}}}.[B]) else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_resolve_all_components()
            {
                Assert.AreEqual(2, _model.Components.Count);
                Assert.IsTrue(_model.Components.Any(x => x.ComponentName == Component1));
                Assert.IsTrue(_model.Components.Any(x => x.ComponentName == Component2));
            }

            [TestMethod]
            public void Should_generate_sql_containing_all_tables()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains($"[{Component1}].[Id]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains($"[{Component2}].[Id]"));
            }
        }

        [TestClass]
        public class WhenIncludingForAllComponentId
        {
            private Model _model;
            private readonly string _text = "ruleset Foo rule 100.0 when {A} exists then require that {B} exist else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_include_both_tables()
            {
                Assert.IsTrue(_model.ComponentNames.Any(x => x == "A"));
                Assert.IsTrue(_model.ComponentNames.Any(x => x == "B"));
            }

        }

        [TestClass]
        public class WhenConstructingFilteredRulesWithComponents
        {
            private Model _model;
            private const string Component1 = "Component1";
            private const string Component2 = "Component2";
            private readonly string _text = $@"ruleset Foo rule 100.0 
when {{{Component1}}}.[NumberCharacteristic1] > 0 then 
require {{{Component2}}}.[DateCharacteristic2] = 10-Jun else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_resolve_all_components()
            {
                Assert.AreEqual(2, _model.Components.Count);
                Assert.IsTrue(_model.Components.Any(x => x.ComponentName == Component1));
                Assert.IsTrue(_model.Components.Any(x => x.ComponentName == Component2));
            }

            [TestMethod]
            public void Should_generate_sql_containing_all_tables()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.IsTrue(_model.Rules[0].Sql.Contains($"[{Component1}].[Id]"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains($"[{Component2}].[Id]"));
            }
        }

        [TestClass]
        public class WhenConstructingRulesWithComponents
        {
            private Model _model;
            private string _text = "ruleset Foo rule 100.0 require {A}.[B,C] is unique else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_not_have_duplicate_tables_in_sql_join()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.AreEqual(1, Regex.Matches(_model.Rules[0].Sql, @"SELECT \[Id\] FROM \[dbo\]\.\[A\]").Count);
                Assert.AreEqual(1, Regex.Matches(_model.Rules[0].Sql, @"LEFT OUTER JOIN \[dbo\]\.\[A\] ON \[Ids\].\[Id\] = \[A\].\[Id\]").Count);
            }
        }
    }
}
