using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IValidatedDataSubmissionService
    {
        IEnumerable<SchoolYear> GetYearsOpenForDataSubmission();
    }
}
