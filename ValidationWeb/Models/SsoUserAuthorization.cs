using System.Text;

namespace ValidationWeb.Models
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
        //public string FormattedOrganizationId { get; set; }

        // Example:  241
        public int? DistrictNumber { get; set; }

        // Example:  Albert Lee Public School District
        public string OrganizationName { get; set; }

        // 1
        //public int? DistrictType { get; set; }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"UserId: {UserId}\r\n");
            sb.Append($"FirstName: {FirstName}\r\n");
            sb.Append($"MiddleName: {MiddleName}\r\n");
            sb.Append($"LastName: {LastName}\r\n");
            sb.Append($"FullName: {FullName}\r\n");
            sb.Append($"Email: {Email}\r\n");
            sb.Append($"AppId: {AppId}\r\n");
            sb.Append($"AppName: {AppName}\r\n");
            sb.Append($"RoleId: {RoleId}\r\n");
            sb.Append($"RoleDescription: {RoleDescription}\r\n");
            sb.Append($"StateOrganizationId: {StateOrganizationId}\r\n");
            //sb.Append($"FormattedOrganizationId: {FormattedOrganizationId}\r\n");
            sb.Append($"DistrictNumber: {DistrictNumber}\r\n");
            sb.Append($"OrganizationName: {OrganizationName}\r\n");
            //sb.Append($"DistrictType: {DistrictType}\r\n");
            return sb.ToString();
        }
    }
}