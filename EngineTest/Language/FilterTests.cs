using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Models;
using Engine.Utility;

namespace EngineTest.Language
{
    public class FilterTests
    {
        [TestClass]
        public class WhenTestingCollectionEquality
        {
            private Model _model;

            private readonly string _text = $"collection TestCollection1 includes Ruleset1 " +
                                            $"ruleset Ruleset1 " +
                                            $"rule 100.1 when collection is TestCollection1 then require {{A}}.[B] = 1 else 'error' " +
                                            $"rule 100.2 when collection is in [TestCollection1, TestCollection2] then require {{A}}.[B] = 1 else 'error'" +
                                            "rule 100.3 when collection is TestCollection1 and {A}.[B] = {A}.[C] then require {A}.[D] = 1 else 'error'";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_generate_where_clause_for_collection_is()
            {
                var sql = _model.Rules[0].Sql;
                Console.WriteLine(sql);
                Assert.IsTrue(sql.Contains("@collectionId = 'TestCollection1'"));
            }

            [TestMethod]
            public void Should_generate_where_clause_for_collection_is_in()
            {
                var sql = _model.Rules[1].Sql;
                Console.WriteLine(sql);
                Assert.IsTrue(sql.Contains("@collectionId IN ('TestCollection1', 'TestCollection2')"));
            }

            [TestMethod]
            public void Should_generate_compund_where_clause()
            {
                var sql = _model.Rules[2].Sql;
                Console.WriteLine(sql);
                Assert.IsTrue(sql.Contains("@collectionId = 'TestCollection1'"));
                Assert.IsTrue(sql.Contains("([A].[D] = 1)"));
            }
        }
    }
}
