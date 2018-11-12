using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class StudentDrillDownViewModel
    {
        public string StudentId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictNumber { get; set; }
        public string EnrolledDate { get; set; }
        public string WithdrawnDate { get; set; }
    }
}