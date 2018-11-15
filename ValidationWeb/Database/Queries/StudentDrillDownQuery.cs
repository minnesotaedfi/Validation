using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class StudentDrillDownQuery
    {
        public const string StudentIdColumnName = "StudentId";
        public const string StudentFirstNameColumnName = "StudentFirstName";
        public const string StudentMiddleNameColumnName = "StudentMiddleName";
        public const string StudentLastNameColumnName = "StudentLastName";
        public const string DistrictIdColumnName = "DistrictId";
        public const string DistrictNameColumnName = "DistrictName";
        public const string SchoolIdColumnName = "SchoolId";
        public const string SchoolNameColumnName = "SchoolName";
        public const string EnrolledDateColumnName = "EnrolledDate";
        public const string WithdrawDateColumnName = "WithdrawDate";
        public const string GradeColumnName = "Grade";
        public const string SpecialEdStatusColumnName = "SpecialEdStatus";
        public string StudentId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int? SchoolId { get; set; }
        public string SchoolName { get; set; }
        public DateTime? EnrolledDate { get; set; }
        public DateTime? WithdrawDate { get; set; }
        public string Grade { get; set; }
        public string SpecialEdStatus { get; set; }
    }
}