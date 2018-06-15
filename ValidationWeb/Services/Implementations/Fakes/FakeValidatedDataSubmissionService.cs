using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeValidatedDataSubmissionService : IValidatedDataSubmissionService
    {
        public IEnumerable<SchoolYear> GetYearsOpenForDataSubmission()
        {
            return new List<SchoolYear>
            {
                new SchoolYear {Id = 0, StartYear = "2018", EndYear = "2019" },
                new SchoolYear {Id = 0, StartYear = "2019", EndYear = "2020" },
            };
        }
    }
}