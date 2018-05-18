using System;
using System.Linq;
using Engine.Language;
using Engine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Models
{
    public class ModelTests
    {
        [TestClass]
        public class WhenPopulatingTheModel
        {
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Should_not_allow_duplicate_collections()
            {
                const string collectionId = "Collection1";
                var model = new Model();
                var collection1 = new Collection(collectionId);
                model.AddCollection(collection1);
                var collection2 = new Collection(collectionId);
                model.AddCollection(collection2);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Should_not_allow_duplicate_rules()
            {
                const string rulesetId = "Ruleset1";
                const string ruleId = "100.0";
                var schemaProvider = new SchemaProvider();
                var model = new Model();
                var rule1 = new Rule(ruleId, rulesetId, schemaProvider);
                model.AddRule(rule1);
                var rule2 = new Rule(ruleId, rulesetId, schemaProvider);
                model.AddRule(rule2);
            }

            [TestMethod]
            public void Should_include_collectionId_as_parameter()
            {
                const string collectionId = "test";
                var model = new Model();
                var parameters = model.GetParameters(collectionId);
                Assert.AreEqual(collectionId, parameters[0].Value.ToString());
            }

            [TestMethod]
            public void Should_combine_global_and_collection_parameters()
            {
                const string collectionId = "test";
                var model = new Model();
                model.AddParameter(string.Empty, new System.Data.SqlClient.SqlParameter("@global", 1));
                model.AddParameter(collectionId, new System.Data.SqlClient.SqlParameter("@local", 2));
                var result = model.GetParameters(collectionId);
                Assert.IsTrue(result.Count() == 3);
                Assert.IsTrue(result.Any(x => x.Value.ToString() == "1"));
                Assert.IsTrue(result.Any(x => x.Value.ToString() == "2"));
            }

            [TestMethod]
            public void Should_override_global_parameters()
            {
                const string collectionId = "test";
                var model = new Model();
                model.AddParameter(string.Empty, new System.Data.SqlClient.SqlParameter("@a", 1));
                model.AddParameter(collectionId, new System.Data.SqlClient.SqlParameter("@a", 2));
                var result = model.GetParameters(collectionId);
                Assert.IsTrue(result.Count() == 2);
                Assert.IsFalse(result.Any(x => x.Value.ToString() == "1"));
                Assert.IsTrue(result.Any(x => x.Value.ToString() == "2"));
            }

            [TestMethod]
            public void Should_overwrite_previous_values()
            {
                const string collectionId = "test";
                var model = new Model();
                model.AddParameter(string.Empty, new System.Data.SqlClient.SqlParameter("@a", 1));
                model.AddParameter(string.Empty, new System.Data.SqlClient.SqlParameter("@a", 2));
                var result = model.GetParameters(collectionId);
                Assert.IsTrue(result.Count() == 2);
                Assert.IsFalse(result.Any(x => x.Value.ToString() == "1"));
                Assert.IsTrue(result.Any(x => x.Value.ToString() == "2"));
            }
        }
    }
}
