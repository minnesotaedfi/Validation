namespace ValidationWeb.Services.Interfaces
{
    public interface IOdsConfigurationValues
    {
        string GetRawOdsConnectionString(string fourDigitYear);

        string GetValidatedOdsConnectionString(string fourDigitYear);
    }
}