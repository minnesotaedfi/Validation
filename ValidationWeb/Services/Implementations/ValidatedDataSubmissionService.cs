using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class ValidatedDataSubmissionService : IValidatedDataSubmissionService
    {
        public IEnumerable<SchoolYear> GetYearsOpenForDataSubmission()
        {
            return new List<SchoolYear>
            {
                new SchoolYear("2018", "2019"),
                new SchoolYear("2019", "2020")
            };
        }
    }
}