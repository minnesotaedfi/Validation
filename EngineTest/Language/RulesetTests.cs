using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Models;
using Engine.Utility;

namespace EngineTest.Language
{
    public class RulesetTests
    {
        [TestClass]
        public class WhenParsingRuleset
        {
            private Model _model;

            private const string RuleSetId1 = "TestRuleset1";
            private const string RuleId1 = "100.0.1";
            private readonly string _text = $@"ruleset {RuleSetId1} rule {RuleId1} require {{A}}.[B] exist else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_add_rulesetId_to_rule()
            {
                Assert.AreEqual(1, _model.Rules.Count);
                Assert.AreEqual(RuleId1, _model.Rules[0].RuleId);
                Assert.AreEqual(RuleSetId1, _model.Rules[0].RulesetId);
            }
        }

        [TestClass]
        public class WhenParsingRulesetWithMultipleRules
        {
            private Model _model;

            private const string RuleSetId1 = "TestRuleset1";
            private const string RuleId1 = "100.0.1";
            private const string RuleId2 = "100.0.2";
            private readonly string _text = $@"
ruleset {RuleSetId1} 
rule {RuleId2} require that {{A}}.[B] exist else ""error""
rule {RuleId1} expect that {{A}}.[B] exists else ""warning""";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_add_rulesetIds_to_rule()
            {
                Assert.AreEqual(2, _model.Rules.Count);
            }

            [TestMethod]
            public void Should_add_rulesetId_to_second_rule()
            {
                Assert.AreEqual(RuleId1, _model.Rules[1].RuleId);
                Assert.AreEqual(RuleSetId1, _model.Rules[1].RulesetId);
            }
        }
    }
}