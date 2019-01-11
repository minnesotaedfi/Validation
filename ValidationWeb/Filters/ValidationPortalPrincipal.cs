using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;

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
            var roleSought = AppRole.CreateAppRole(role);
            //return portalIdentity.AppRole >= roleSought;
            return portalIdentity.AppRole.Name == roleSought.Name;
        }
    }

    /// <summary>
    /// A representation of the authenticated Validation Portal user, for use in HTTP Contexts.
    /// </summary>
    [Serializable]
    public class ValidationPortalIdentity : IIdentity
    {
        public string AuthenticationType { get { return "Minnesota DOE SSO"; } }
        public bool IsAuthenticated { get { return true; } }
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

    /// <summary>
    /// Extensions for IIdentity
    /// </summary>
    public static class IIdentityExtensions
    {

        /// <summary>
        /// Permission names for the portal application.
        /// </summary>                    
        private static class PortalPermissions
        {
            public const string CanViewAllAdminFunctions = "CanViewAllAdminFunctions";                                          // EDVP-Admin -- true   EDVP-HelpDesk -- false   EDVP-RegionUser -- false     EDVP-DistrictUser -- false      EDVP-DataOwner -- false 
            public const string CanViewStudentCounts = "CanViewStudentCounts";                                                  // EDVP-Admin -- false  EDVP-HelpDesk -- true    EDVP-RegionUser -- true      EDVP-DistrictUser -- true       EDVP-DataOwner -- false
            public const string CanViewReadOnlyAllFeaturesAllDistricts = "CanViewReadOnlyAllFeaturesAllDistricts";              // EDVP-Admin -- false  EDVP-HelpDesk -- true    EDVP-RegionUser -- false     EDVP-DistrictUser -- false      EDVP-DataOwner -- false
            public const string CanViewODSReportsViewAllDistricts = "CanViewODSReportsViewAllDistricts";                        // EDVP-Admin -- false  EDVP-HelpDesk -- true    EDVP-RegionUser -- true      EDVP-DistrictUser -- true       EDVP-DataOwner -- false
            public const string CanViewODSReportLinks = "CanViewODSReportsLinks";                                               // EDVP-Admin -- false  EDVP-HelpDesk -- false   EDVP-RegionUser -- true      EDVP-DistrictUser -- true       EDVP-DataOwner -- false
            public const string CanViewAllAggregatedReportsForAllDistricts = "CanViewAllAggregatedReportsForAllDistricts";      // EDVP-Admin -- false  EDVP-HelpDesk -- true    EDVP-RegionUser -- true      EDVP-DistrictUser -- true       EDVP-DataOwner -- true
            public const string CanViewAllValidationReportsTab = "CanViewAllValidationReportsTab";                              // EDVP-Admin -- false  EDVP-HelpDesk -- true    EDVP-RegionUser -- true      EDVP-DistrictUser -- true       EDVP-DataOwner -- false
            public const string CanViewAllDistricts = "CanViewAllDistricts";                                                    // EDVP-Admin -- false  EDVP-HelpDesk -- true    EDVP-RegionUser -- false     EDVP-DistrictUser -- false      EDVP-DataOwner -- true

        }

        private static class PortalRoles
        {
            public const string Admin = "Administrator";
            public const string EDVPAdmin = "EDVP-Admin";
            public const string EDVPHelpDesk = "EDVP-HelpDesk";
            public const string EDVPRegionUser = "EDVP-RegionUser";
            public const string EDVPDistrictUser = "EDVP-DistrictUser";
            public const string EDVPDataOwner = "EDVP-DataOwner";
        }

        /// <summary>
        /// Returns the app role name so it's easier to get at.
        /// </summary>
        public static string AppRoleName { get; set; }

        /// <summary>
        /// Method to extend IIdentity so that it will return the dictionary of permissions
        /// </summary>
        /// <param name="identity">ValidationPortalIdentity</param>
        /// <param name="roleName">Role Name of the User</param>
        /// <returns>Dictionary with permissions and the settings for that user.</returns>
        public static Dictionary<string, bool> GetViewPermissions(this IIdentity identity, string roleName) //ILoggingService _loggingService
        {

            Dictionary<string, bool> permissions = new Dictionary<string, bool>();

            // Start with every permission being automatically set to false;
            permissions.Add(PortalPermissions.CanViewAllAdminFunctions, false);
            permissions.Add(PortalPermissions.CanViewStudentCounts, false);
            permissions.Add(PortalPermissions.CanViewAllValidationReportsTab, false);

            // Logic for setting the permissions.  TODO: needs to be a switch
            switch (roleName)
            {
                case PortalRoles.Admin:
                    permissions[PortalPermissions.CanViewAllAdminFunctions] = true;
                    permissions[PortalPermissions.CanViewStudentCounts] = true;
                    permissions[PortalPermissions.CanViewAllValidationReportsTab] = false;
                    break;
                case PortalRoles.EDVPAdmin:
                    permissions[PortalPermissions.CanViewAllAdminFunctions] = true;
                    permissions[PortalPermissions.CanViewStudentCounts] = true;
                    permissions[PortalPermissions.CanViewAllValidationReportsTab] = false;
                    break;
                default:
                    break;
            }

            return permissions;
        }
    }
}



