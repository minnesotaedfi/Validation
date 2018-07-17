using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface IRulesEngineConfigurationValues
    {
        string RulesFileFolder { get; }
        string RuleEngineResultsSchema { get; }
        string RuleEngineResultsConnectionString { get;  }
    }
}