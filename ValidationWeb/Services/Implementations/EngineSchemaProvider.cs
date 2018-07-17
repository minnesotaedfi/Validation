using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Engine.Language;

namespace ValidationWeb.Services
{
    public class EngineSchemaProvider : ISchemaProvider
    {
        private static string _ruleEngineResultsSchema;
        static EngineSchemaProvider()
        {
            _ruleEngineResultsSchema = (new RulesEngineConfigurationValues()).RuleEngineResultsSchema;
        }

        public string Value
        {
            get
            {
                return _ruleEngineResultsSchema;
            }
        }
    }
}