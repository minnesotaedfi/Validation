using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
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