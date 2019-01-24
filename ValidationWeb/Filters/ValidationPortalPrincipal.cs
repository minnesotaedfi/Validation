using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace ValidationWeb
{
    public class ValidationPortalPrincipal : IPrincipal
    {
        public ValidationPortalPrincipal(ValidationPortalIdentity ident)
        {
            Identity = ident;
        }
        public IIdentity Identity { get; set; }

        public ValidationPortalIdentity PortalIdentity()
        {
            var portalIdentity = Identity as ValidationPortalIdentity;
            return portalIdentity;
        }

        public bool IsInRole(string role)
        {
            var portalIdentity = Identity as ValidationPortalIdentity;
            if (portalIdentity == null)
            {
                return false;
            }
            
            return string.CompareOrdinal(portalIdentity.AppRole.Name, role) == 0;
        }
    }

    /// <summary>
    /// A representation of the authenticated Validation Portal user, for use in HTTP Contexts.
    /// </summary>
    [Serializable]
    public class ValidationPortalIdentity : IIdentity
    {
        public string AuthenticationType => "Minnesota DOE SSO";

        public bool IsAuthenticated => true;

        public string Name { get; set; }

        public AppRole AppRole { get; set; }

        [Key]
        public string UserId { get; set; }
        
        public string FirstName { get; set; }
        
        public string MiddleName { get; set; }
        
        public string LastName { get; set; }
        
        public string FullName { get; set; }
        
        public string Email { get; set; }
        
        public ICollection<EdOrg> AuthorizedEdOrgs { get; set; }
    }

    public class SSORoleNames
    {
        public const string Admin = "EDVP-Admin";
        public const string HelpDesk = "EDVP-HelpDesk";
        public const string RegionUser = "EDVP-RegionUser";
        public const string DistrictUser = "EDVP-DistrictUser";
        public const string DataOwner = "EDVP-DataOwner";
    }

    public class PortalRoleNames
    {
        public const string Admin = "Administrator";
        
        public const string HelpDesk = "HelpDesk";
        
        public const string RegionUser = "RegionUser";

        public const string DistrictUser = "DistrictUser";

        public const string DataOwner = "DataOwner";
    }

    public class RolePermissions
    {
        public bool CanAccessAdminFeatures { get; set; }

        public bool CanViewOdsReports { get; set; }

        public bool CanModifyRecordsRequests { get; set; }

        public bool CanViewStudentDrilldownReports { get; set; }

        public bool CanViewValidationReports { get; set; }

        public bool CanRunValidationReports { get; set; }

        public bool CanAccessAllDistrictsMode { get; set; }
    }

    /// <summary>
    /// Extensions for IIdentity
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IIdentityExtensions
    {
        /// <summary>
        /// Method to extend IIdentity so that it will return the dictionary of permissions
        /// </summary>
        /// <param name="identity">ValidationPortalIdentity</param>
        /// <param name="roleName">Role Name of the User</param>
        /// <returns>Dictionary with permissions and the settings for that user.</returns>
        public static RolePermissions GetViewPermissions(this IIdentity identity, AppRole role) 
        {
            var rolePermissions = new RolePermissions();

            switch (role.Name)
            {
                case PortalRoleNames.Admin:
                    rolePermissions.CanAccessAdminFeatures = true; 
                    break;
                case PortalRoleNames.HelpDesk:
                    rolePermissions.CanViewOdsReports = true;
                    rolePermissions.CanViewStudentDrilldownReports = true;
                    rolePermissions.CanViewValidationReports = true;
                    rolePermissions.CanAccessAllDistrictsMode = true; 
                    break; 
                case PortalRoleNames.DataOwner:
                    rolePermissions.CanViewOdsReports = true;
                    rolePermissions.CanAccessAllDistrictsMode = true; 
                    break; 
                case PortalRoleNames.DistrictUser:
                    rolePermissions.CanViewOdsReports = true;
                    rolePermissions.CanModifyRecordsRequests = true;
                    rolePermissions.CanViewStudentDrilldownReports = true;
                    rolePermissions.CanViewValidationReports = true;
                    rolePermissions.CanRunValidationReports = true;
                    break;
                case PortalRoleNames.RegionUser:
                    rolePermissions.CanViewOdsReports = true;
                    rolePermissions.CanModifyRecordsRequests = true;
                    rolePermissions.CanViewStudentDrilldownReports = true;
                    rolePermissions.CanViewValidationReports = true;
                    rolePermissions.CanRunValidationReports = true; 
                    break; 
            }

            return rolePermissions;
        }
    }
}



