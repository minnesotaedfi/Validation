using System;
using Engine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Models
{
    public class CollectionTests
    {
        [TestClass]
        public class WhenPopulatingCollections
        {
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Should_not_allow_duplicate_ruleIds()
            {
                const string collectionId = "Collection1";
                const string ruleId = "100.0";
                var collection = new Collection(collectionId);
                collection.AddRuleReference(ruleId);
                collection.AddRuleReference(ruleId);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Should_not_allow_duplicate_rulesetIds()
            {
                const string collectionId = "Collection1";
                const string rulesetId = "Ruleset1";
                var collection = new Collection(collectionId);
                collection.AddRulesetReference(rulesetId);
                collection.AddRulesetReference(rulesetId);
            }
        }
    }
}
