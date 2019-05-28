using System.Collections.Generic;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class ValidatedDataSubmissionService : IValidatedDataSubmissionService
    {
        private readonly ISchoolYearService _schoolYearService;

        public ValidatedDataSubmissionService(ISchoolYearService schoolYearService)
        {
            _schoolYearService = schoolYearService;
        }

        public IEnumerable<SchoolYear> GetYearsOpenForDataSubmission()
        {
            return _schoolYearService.GetSubmittableSchoolYears();
        }
    }
}