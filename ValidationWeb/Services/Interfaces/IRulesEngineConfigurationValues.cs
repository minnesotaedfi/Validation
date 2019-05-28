namespace ValidationWeb.Services.Interfaces
{
    public interface IRulesEngineConfigurationValues
    {
        string RulesFileFolder { get; }

        string RuleEngineResultsSchema { get; }

        string RuleEngineResultsConnectionString { get; }
    }
}