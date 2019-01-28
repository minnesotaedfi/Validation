﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    using ValidationWeb.Filters;

    public class OdsResidentsEnrolledElsewhereReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }
        public int EdOrgId { get; set; }
        public string EdOrgName { get; set; }
        public bool IsStateMode { get; set; }
        public string FourDigitSchoolYear { get; set; }
    }
}