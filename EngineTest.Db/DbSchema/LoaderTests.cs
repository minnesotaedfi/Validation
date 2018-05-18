using System.Linq;
using System.Collections.Generic;
using Engine.DbSchema;
using Engine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Db.DbSchema
{
    public class LoaderTests
    {
        [TestClass]
        public class WhenLoaderIsLoadedFromSampleDatabase
        {
            private IReadOnlyList<Column> _tables;
            private IReadOnlyList<Parameter> _functions;

            [TestInitialize]
            public void TestInitialize()
            {
                using (var context = new EngineDbTestContext())
                {
                    var loader = new Loader(context.Database.Connection);
                    _tables = loader.Tables;
                    _functions = loader.Functions;
                }
            }

            [TestMethod]
            public void Source_tables_should_exist()
            {
                Assert.IsTrue(_tables.Any(x => x.TableName == "Component1"));
                Assert.IsTrue(_tables.Any(x => x.TableName == "Component2"));
            }

            [TestMethod]
            public void Columns_should_exist()
            {
                Assert.IsTrue(_tables.Any(x => x.ColumnName == "StringCharacteristic1"));
                Assert.IsTrue(_tables.Any(x => x.ColumnName == "NumberCharacteristic1"));
                Assert.IsTrue(_tables.Any(x => x.ColumnName == "DateCharacteristic1"));
                Assert.IsTrue(_tables.Any(x => x.ColumnName == "BoolCharacteristic1"));
            }

            [TestMethod]
            public void Functions_should_exist()
            {
                // This test should be updated if there are any functions added to the Engine.Db database project
                Assert.IsFalse(_functions.Any());
            }
        }

        [TestClass]
        public class WhenValidatingTheSampleDatabase
        {
            private IReadOnlyList<Validator.ValidationError> _errors;

            [TestInitialize]
            public void TestInitialize()
            {
                using (var context = new EngineDbTestContext())
                {
                    var model = new Model();

                    // valid tables and columns
                    model.AddComponent("Component1");
                    model.AddComponent("Component2", "StringCharacteristic2");
                    model.AddComponent("Component2", "BoolCharacteristic2");

                    // invalid
                    model.AddComponent("InvalidTable1");
                    model.AddComponent("Component1", "InvalidColumn1");
                    model.AddComponent("InvalidTable2", "InvalidColumn2");
                    model.AddFunction("InvalidFunction1");

                    var loader = new Loader(context.Database.Connection);

                    var validator = new Validator(model, loader);
                    _errors = validator.Validate();
                }
            }

            [TestMethod]
            public void Should_have_errors_for_unknown_table()
            {
                Assert.IsTrue(_errors.Any(x => x.Text.Contains("InvalidTable1")));
                Assert.IsTrue(_errors.Any(x => x.Text.Contains("InvalidTable2")));
            }

            [TestMethod]
            public void Should_have_errors_for_unknown_column()
            {
                Assert.IsTrue(_errors.Any(x => x.Text.Contains("InvalidColumn1")));
                Assert.IsTrue(_errors.Any(x => x.Text.Contains("InvalidColumn2")));
            }

            [TestMethod]
            public void Should_have_errors_for_unknown_function()
            {
                Assert.IsTrue(_errors.Any(x => x.Text.Contains("InvalidFunction1")));
            }

            [TestMethod]
            public void Should_not_have_unanticipated_errors()
            {
                Assert.AreEqual(5, _errors.Count);
            }
        }
    }
}