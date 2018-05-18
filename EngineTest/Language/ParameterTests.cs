using System.Linq;
using Engine.Models;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Language
{
    public class ParameterTests
    {
        [TestClass]
        public class WhenUsingGlobalParameters
        {
            private Model _model;
            private const string Param1Name = "Param1";
            private const decimal Param1Value = 1;
            private const decimal Param1Override = 2;
            private const string Param2Name = "Param2";
            private const decimal Param2Value = 1;
            private const string CollectionId = "TestCollection";
            private readonly string _text = $"define {Param1Name} = {Param1Value},  {Param2Name} = {Param2Value}" +
                                            $"collection {CollectionId} includes 100.1 " +
                                            $"defines {Param1Name} = {Param1Override} " +
                                            $"ruleset Foo " +
                                            $"rule 100.1 require that {{A}}.[B] = {Param1Name} else 'error' " +
                                            $"rule 100.2 require that {{A}}.[B] = {Param2Name} else 'error' ";

            [TestInitialize]
            public void Setup()
            {
                var stream = _text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Require_sql_to_include_parameter()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains($"@{Param1Name}"));
                Assert.IsTrue(_model.Rules[1].Sql.Contains($"@{Param2Name}"));
            }


            [TestMethod]
            public void Require_parameter_to_be_created()
            {
                var parameters = _model.GetParameters(CollectionId);
                Assert.AreEqual(3, parameters.Length);
                Assert.AreEqual(Param1Override, parameters.Single(x => x.ParameterName == $"@{Param1Name}").Value);
                Assert.AreEqual(Param2Value, parameters.Single(x => x.ParameterName == $"@{Param2Name}").Value);
            }
        }
    }
}
