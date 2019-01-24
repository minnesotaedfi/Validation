namespace ValidationWeb.Services.Implementations
{
    using System.Collections.Generic;

    public interface IEdFiApiLogService
    {
        IEnumerable<Log> GetIdentityIssues();

        IEnumerable<Log> GetApiErrors();
    }
}