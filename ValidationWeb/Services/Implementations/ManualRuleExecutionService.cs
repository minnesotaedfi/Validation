using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class ManualRuleExecutionService: IManualRuleExecutionService
    {
        private readonly IRulesEngineConfigurationValues _rulesEngineConfigurationValues;
        public ManualRuleExecutionService(IRulesEngineConfigurationValues rulesEngineConfigurationValues)
        {
            _rulesEngineConfigurationValues = rulesEngineConfigurationValues;
        }
        public async Task<List<string>> GetManualSqlFile(string ruleSetId, string ruleId)
        {
            var readingDirectory = $"{_rulesEngineConfigurationValues.SqlRulesFileFolder}/{ruleSetId}";

            if (ruleSetId == "StudentEnrollment")
            {
                Console.WriteLine("Ok");
            }

            var manualInstructionsToExecute = new List<string>();

            if (Directory.Exists(readingDirectory))
            {
                foreach (var fileName in Directory.EnumerateFiles(readingDirectory))
                {
                    if (fileName.Contains(fileName))
                    {
                        using (var reader = File.OpenText(fileName))
                        {
                            var fileSqlExec = await reader.ReadToEndAsync();
                            manualInstructionsToExecute.Add(fileSqlExec);
                        }
                    }
                }
            }

            return manualInstructionsToExecute;
        }
    }
}