using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IEdFiApiLogService
    {
        IEnumerable<Log> GetIdentityIssues();

        IEnumerable<Log> GetApiErrors();
    }
}