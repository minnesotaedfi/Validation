using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Engine.Models
{
    public class Model
    {
        private readonly List<Collection> _collections = new List<Collection>();
        private readonly List<Rule> _rules = new List<Rule>();
        private readonly List<Component> _components = new List<Component>();
        private readonly List<Function> _functions = new List<Function>();
        private readonly Dictionary<string, List<SqlParameter>> _parameters = new Dictionary<string, List<SqlParameter>>();

        public IReadOnlyList<Collection> Collections => _collections;
        public IReadOnlyList<Component> Components => _components.Where(x => !string.IsNullOrEmpty(x.CharacteristicName)).Distinct().ToList();
        public IReadOnlyList<string> ComponentNames => _components.Select(x => x.ComponentName).Distinct().ToList();
        public IReadOnlyList<Rule> Rules => _rules;
        public IReadOnlyList<Function> Functions => _functions;

        public void AddCollection(Collection collection)
        {
            if (string.IsNullOrEmpty(collection.CollectionId) || _collections.Any(x => x.CollectionId == collection.CollectionId))
                throw new ArgumentException($"Duplicate or empty CollectionId: {collection.CollectionId}", nameof(collection));
            collection.Model = this;
            _collections.Add(collection);
        }

        public void AddComponent(string componentName, string characteristicName = null)
        {
            if (string.IsNullOrEmpty(componentName))
                throw new ArgumentException("empty component name", nameof(componentName));
            if (!_components.Any(x => x.ComponentName == componentName && x.CharacteristicName == characteristicName))
                _components.Add(new Component(componentName, characteristicName));
        }

        public void AddFunction(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentException("empty function name", nameof(functionName));
            if (_functions.All(x => x.FunctionName != functionName))
                _functions.Add(new Function(functionName));
        }

        public void AddRule(Rule rule)
        {
            if (string.IsNullOrEmpty(rule.RuleId) || 
                _rules.Any(x => x.RuleId == rule.RuleId && x.RulesetId == rule.RulesetId))
            {
                throw new ArgumentException($"Duplicate RuleId: {rule.RuleId}", nameof(rule));
            }

            _rules.Add(rule);
        }

        /// <summary>
        /// Return an individual rule, all rules in a ruleset, or all rules in a collection
        /// </summary>
        /// <param name="id">CollectionId, RulesetId, or RuleId</param>
        /// <returns>all the matching rules</returns>
        public IEnumerable<Rule> GetRules(string id)
        {
            var collection = _collections.SingleOrDefault(x => x.CollectionId == id) ?? new Collection(string.Empty);

            var result = _rules.Where(x =>
                x.RuleId == id || collection.RuleIds.Contains(x.RuleId) ||
                x.RulesetId == id || collection.RulesetIds.Contains(x.RulesetId));

            return result;
        }

        public void AddParameter(string collectionId, SqlParameter parameter)
        {
            if (!_parameters.ContainsKey(collectionId))
            {
                _parameters.Add(collectionId, new List<SqlParameter> { parameter });
            }
            else
            {
                _parameters[collectionId].RemoveAll(x => x.ParameterName == parameter.ParameterName);
                _parameters[collectionId].Add(parameter);
            }
        }

        public SqlParameter[] GetParameters(string collectionId)
        {
            var result = new List<SqlParameter> { new SqlParameter("@collectionId", collectionId) };
            if (_parameters.ContainsKey(collectionId))
                result.AddRange(_parameters[collectionId]
                    .Select(x => new SqlParameter(x.ParameterName, x.Value))
                    );
            var names = result.Select(x => x.ParameterName);
            if (_parameters.ContainsKey(string.Empty))
                result.AddRange(_parameters[string.Empty]
                    .Where(x => names.All(y => y != x.ParameterName))
                    .Select(x => new SqlParameter(x.ParameterName, x.Value))
                    );
            return result.ToArray();
        }
    }
}
