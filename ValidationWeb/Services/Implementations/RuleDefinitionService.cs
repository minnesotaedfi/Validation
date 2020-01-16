using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Validation.DataModels;

using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class RuleDefinitionService : IRuleDefinitionService
    {
        public RuleDefinitionService(IRulesEngineConfigurationValues rulesEngineConfigurationValues)
        {
            RulesEngineConfigurationValues = rulesEngineConfigurationValues;
        }

        protected IRulesEngineConfigurationValues RulesEngineConfigurationValues { get; }

        public IEnumerable<RulesetDefinition> GetRulesetDefinitions()
        {
            var rulesPath = RulesEngineConfigurationValues.RulesFileFolder;

            var files = Directory.GetFiles(rulesPath, "*.rules")
                .Where(x => !Path.GetFileName(x).Equals("Collections.rules", StringComparison.OrdinalIgnoreCase));

            var result = new List<RulesetDefinition>();

            foreach (var file in files)
            {
                var ruleset = new RulesetDefinition();
                RuleDefinition rule = null;
                foreach (var rawLine in File.ReadAllLines(file))
                {
                    var text = rawLine.Trim();

                    if (text.StartsWith("ruleset"))
                    {
                        ruleset.Name = Regex.Match(text, "ruleset (.*)$").Groups[1].Value;
                        continue;
                    }

                    if (text.StartsWith("rule"))
                    {
                        rule = new RuleDefinition();
                        rule.Id = Regex.Match(text, @"^rule ([\d.]+)").Groups[1].Value;
                    }

                    if (rule != null)
                    {
                        if (text.StartsWith("expect"))
                        {
                            rule.ValidationType = "Warning";
                        }
                        else if (text.StartsWith("require"))
                        {
                            rule.ValidationType = "Error";
                        }
                        else if (text.StartsWith("else"))
                        {
                            rule.Message = Regex.Match(text, @"else '(.*)'").Groups[1].Value;
                            ruleset.RuleDefinitions.Add(rule);
                            rule = null;
                        }
                    }
                }
                result.Add(ruleset);
            }

            return result;
        }
    }
}