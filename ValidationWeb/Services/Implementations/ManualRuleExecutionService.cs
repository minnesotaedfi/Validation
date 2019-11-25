using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
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
            var readingDirectory = $@"{_rulesEngineConfigurationValues.SqlRulesFileFolder}\{ruleSetId}";
            //var readingDirectory = Server. $"~/Content/SqlRules/{ruleSetId}";

            if (ruleSetId == "StudentEnrollment")
            {
                Console.WriteLine("Ok");
            }

            var manualInstructionsToExecute = new List<string>();

            //var directory = Directory.GetCurrentDirectory();
            //Console.WriteLine(directory);

            if (Directory.Exists(readingDirectory))
            {
                foreach (var fileName in Directory.EnumerateFiles(readingDirectory))
                {
                    if (fileName.Contains(ruleId))
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