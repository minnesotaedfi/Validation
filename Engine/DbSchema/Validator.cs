using System.Linq;
using System.Collections.Generic;
using Engine.Models;

namespace Engine.DbSchema
{
    public class Validator
    {
        public class ValidationError
        {
            public string Text;
        }

        private readonly Model _rulesModel;
        private readonly Loader _dbSchemaLoader;

        public Validator(Model rulesModel, Loader dbSchemaLoader)
        {
            _rulesModel = rulesModel;
            _dbSchemaLoader = dbSchemaLoader;
        }

        public IReadOnlyList<ValidationError> Validate()
        {
            var tables = _dbSchemaLoader.Tables;
            var functions = _dbSchemaLoader.Functions;

            var result = new List<ValidationError>();
            result.AddRange(
                _rulesModel.ComponentNames
                .Where(cn => tables.All(t => t.TableName != cn))
                .Select(cn => new ValidationError { Text = $"Table {cn} does not exist in database" })
                );

            result.AddRange(
                _rulesModel.Components
                .Where(c => !tables.Any(t => t.TableName == c.ComponentName && t.ColumnName == c.CharacteristicName))
                .Select(c => new ValidationError { Text = $"Column [{c.ComponentName}].[{c.CharacteristicName}] does not exist in database" })
                );

            result.AddRange(
                _rulesModel.Functions
                .Where(f => functions.All(x => f.FunctionName != x.FunctionName))
                .Select(f => new ValidationError { Text = $"Function {f.FunctionName} does not exist in database" })
                );
            return result;
        }
    }
}
