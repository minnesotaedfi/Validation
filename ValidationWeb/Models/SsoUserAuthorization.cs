using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    /// <summary>
    /// This class represents a single row in the recordset returned by the Minnesota Dept of Education
    /// application SSO authorization list for the Portal UI application.
    /// </summary>
    public class SsoUserAuthorization
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string RoleId { get; set; }
        public string RoleDescription { get; set; }
        // Example:  10241000000
        public string StateOrganizationId { get; set; }
        // Example:  0241-01
        public string FormattedOrganizationId { get; set; }
        // Example:  241
        public int? DistrictNumber { get; set; }
        // Example:  Albert Lee Public School District
        public string OrganizationName { get; set; }
        // 1
        public int? DistrictType { get; set; }
        // Not used - these comments are documenting the availablity of these properties.
        // public int? SchoolNumber { get; set; }
        // public bool HasParentConcept { get; set; }  Part of a tree - we determine this tree using ODS information.
    }
}