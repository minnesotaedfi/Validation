using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class StudentDataFromId
    {
        public static string StudentDataQueryFromId =
@"SELECT s.[FirstName], s.[MiddleName], s.[LastSurname], 
    eorg.NameOfInstitution, ssa.SchoolId, ssa.EntryDate, ssa.ExitWithdrawDate, 
	d.ShortDescription AS GradeLevel
    FROM edfi.Student s 
    LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
    LEFT OUTER JOIN edfi.School sch ON sch.SchoolId = ssa.SchoolId
    LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
    LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssa.EntryGradeLevelDescriptorId
    WHERE s.StudentUniqueId=@student_unique_id";
        public const string FirstNameColumnName = "FirstName";
        public const string MiddleNameColumnName = "MiddleName";
        public const string LastSurnameColumnName = "LastSurname";
        public const string NameOfInstitutionColumnName = "NameOfInstitution";
        public const string SchoolIdColumnName = "SchoolId";
        public const string EntryDateColumnName = "EntryDate";
        public const string ExitWithdrawDateColumnName = "ExitWithdrawDate";
        public const string GradeLevelColumnName = "GradeLevel";

        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastSurname { get; set; } = string.Empty;
        public string NameOfInstitution { get; set; } = string.Empty;
        public string SchoolId { get; set; } = string.Empty;
        public DateTime? EntryDate { get; set; } = null;
        public DateTime? ExitWithdrawDate { get; set; } = null;
        public string GradeLevel { get; set; } = string.Empty;
        public static string GetStudentFullName(IList<StudentDataFromId> singleStudentDataList)
        {
            if (singleStudentDataList.Count == 0)
            {
                return string.Empty;
            }
            return (string.IsNullOrWhiteSpace(singleStudentDataList[0].MiddleName)
                ? $"{singleStudentDataList[0].FirstName} {singleStudentDataList[0].LastSurname}"
                : $"{singleStudentDataList[0].FirstName} {singleStudentDataList[0].MiddleName} {singleStudentDataList[0].LastSurname}")
                .Trim();
        }
        public static List<string> GetSchools(IList<StudentDataFromId> singleStudentDataList)
        {
            if (singleStudentDataList.Count == 0)
            {
                return new List<string>();
            }
            return singleStudentDataList.Select(ssd => ssd.NameOfInstitution).ToList();
        }

        public static List<string> GetSchoolIds(IList<StudentDataFromId> singleStudentDataList)
        {
            if (singleStudentDataList.Count == 0)
            {
                return new List<string>();
            }
            return singleStudentDataList.Select(ssd => ssd.SchoolId).ToList();
        }

        public static List<DateTime?> GetDateEnrolledList(IList<StudentDataFromId> singleStudentDataList)
        {
            if (singleStudentDataList.Count == 0)
            {
                return new List<DateTime?>();
            }

            return singleStudentDataList.Select(ssd => ssd.EntryDate).ToList();
        }

        public static List<DateTime?> GetDateWithdrawnList(IList<StudentDataFromId> singleStudentDataList)
        {
            if (singleStudentDataList.Count == 0)
            {
                return new List<DateTime?>();
            }

            return singleStudentDataList.Select(ssd => ssd.ExitWithdrawDate).ToList();
        }

        public static List<string> GetGradeLevels(IList<StudentDataFromId> singleStudentDataList)
        {
            if (singleStudentDataList.Count == 0)
            {
                return new List<string>();
            }
            return singleStudentDataList.Select(ssd => ssd.GradeLevel).ToList();
        }
    }
}