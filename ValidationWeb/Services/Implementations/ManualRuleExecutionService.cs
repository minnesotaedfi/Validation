using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class ManualRuleExecutionService: IManualRuleExecutionService
    {
        private readonly IRulesEngineConfigurationValues _rulesEngineConfigurationValues;
        private readonly ILoggingService _loggingService;

        public ManualRuleExecutionService(IRulesEngineConfigurationValues rulesEngineConfigurationValues, ILoggingService loggingService)
        {
            _rulesEngineConfigurationValues = rulesEngineConfigurationValues;
            _loggingService = loggingService;
        }

        public async Task<List<string>> GetManualSqlFile(string ruleSetId, string ruleId)
        {
            var readingDirectory = $@"{_rulesEngineConfigurationValues.SqlRulesFileFolder}\{ruleSetId}";
            var manualInstructionsToExecute = new List<string>();

            
            if (Directory.Exists(readingDirectory))
            {
                _loggingService.LogInfoMessage($"Looking SQLRules for rule: {ruleId}, rules set: {ruleSetId}.");
                foreach (var fileName in Directory.EnumerateFiles(readingDirectory))
                {
                    if (fileName.Contains(ruleId))
                    {
                        _loggingService.LogInfoMessage($"SQLRules founded for rule: {ruleId}, rules set: {ruleSetId}, Path: {fileName}.");
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