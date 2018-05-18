using Engine.Models;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Language
{
    public class RuleTests
    {
        [TestClass]
        public class WhenRuleHasCollectionFilter
        {
            private Model _model;
            private const string Collection1 = "Collection1";
            private readonly string _text = $@"ruleset Foo rule 100.0 when collection is {Collection1} then require 0=1 else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_have_collection_in_model()
            {
                Assert.AreEqual(1, _model.Rules[0].CollectionIds.Count);
                Assert.IsTrue(_model.Rules[0].CollectionIds.Contains(Collection1));
            }
        }

        [TestClass]
        public class WhenRuleHasCollectionFilterList
        {
            private Model _model;
            private const string Collection1 = "Collection1";
            private const string Collection2 = "Collection2";
            private readonly string _text = $@"ruleset Foo rule 100.0 when collection is in [{Collection1},{Collection2}] then require 0=1 else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_have_both_collections_in_model()
            {
                Assert.AreEqual(2, _model.Rules[0].CollectionIds.Count);
                Assert.IsTrue(_model.Rules[0].CollectionIds.Contains(Collection1));
                Assert.IsTrue(_model.Rules[0].CollectionIds.Contains(Collection2));
            }
        }

    }
}
