using System;
using Engine.Models;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Language
{
    public class FunctionTests
    {
        [TestClass]
        public class WhenParsingRulesWithFunctions
        {
            private Model _model;

            private const string FunctionName0 = "F1";
            private const string FunctionName1 = "F2";
            private const int Parameter1A = 123;
            private const string Parameter1B = "{Aa}.[Bb]";
            private const string Parameter1C = "'constant string'";
            private readonly string _text = $@"ruleset Foo rule 100.0 require that {FunctionName0}() = 
{FunctionName1}({Parameter1A}, {Parameter1B}, {Parameter1C}) else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_find_all_functions()
            {
                Assert.AreEqual(2, _model.Functions.Count);
                Assert.AreEqual(FunctionName0, _model.Functions[0].FunctionName);
                Assert.AreEqual(FunctionName1, _model.Functions[1].FunctionName);
            }

            [TestMethod]
            public void Should_have_all_parameters()
            {
                var sql = _model.Rules[0].Sql;
                Assert.IsTrue(sql.Contains(Parameter1A.ToString()));
                Assert.IsTrue(sql.Contains("[Aa].[Bb]"));
                Assert.IsTrue(sql.Contains(Parameter1C));
            }

            [TestMethod]
            public void Should_retain_parameter_order()
            {
                var sql = _model.Rules[0].Sql;
                Assert.IsTrue(sql.IndexOf(Parameter1A.ToString(), StringComparison.Ordinal) < 
                              sql.IndexOf("[Aa].[Bb]", StringComparison.Ordinal));

                Assert.IsTrue(sql.IndexOf("[Aa].[Bb]", StringComparison.Ordinal) <
                              sql.IndexOf(Parameter1C, StringComparison.Ordinal));
            }
        }
    }
}
