using System.Collections.Generic;
using System.Threading.Tasks;

namespace ValidationWeb.Services.Interfaces
{
    public interface IManualRuleExecutionService
    {
        Task<List<string>> GetManualSqlFile(string ruleSetId, string ruleId);
    }
}