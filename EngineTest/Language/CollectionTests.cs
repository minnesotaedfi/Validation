using Engine.Models;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Language
{
    public class CollectionTests
    {
        [TestClass]
        public class WhenParsingCollectionDeclaration
        {
            private Model _model;

            private const string CollectionId = "TestCollection";
            private const string RuleSetId1 = "TestRuleset1";
            private const string RuleSetId2 = "TestRuleset2";
            private const string RuleId1 = "100.0.1";
            private const string RuleId2 = "100.0";
            private readonly string _text = $@"collection {CollectionId} includes {RuleSetId1}, {RuleId1}, {RuleSetId2}, {RuleId2}";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_parse_collectionId()
            {
                Assert.AreEqual(1, _model.Collections.Count);
                Assert.AreEqual(CollectionId, _model.Collections[0].CollectionId);
            }

            [TestMethod]
            public void Should_parse_rulesetIds()
            {
                Assert.AreEqual(2, _model.Collections[0].RulesetIds.Count);
                Assert.AreEqual(RuleSetId1, _model.Collections[0].RulesetIds[0]);
            }

            [TestMethod]
            public void Should_parse_ruleIds()
            {
                Assert.AreEqual(2, _model.Collections[0].RuleIds.Count);
                Assert.AreEqual(RuleId1, _model.Collections[0].RuleIds[0]);
            }
        }
    }
}
