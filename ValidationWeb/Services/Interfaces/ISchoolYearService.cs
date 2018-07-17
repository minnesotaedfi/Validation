﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface ISchoolYearService
    {
        IList<SchoolYear> GetSubmittableSchoolYears();
        void SetSubmittableSchoolYears(IEnumerable<SchoolYear> years);
    }
}