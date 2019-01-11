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
        public Dictionary<string, bool> ViewPermissions { get { return SetViewPermissions(AppRole); } }


        private static class ViewPermissionsConstants
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

        public static Dictionary<string, bool> SetViewPermissions(AppRole appRole) //ILoggingService _loggingService
        {

            Dictionary<string, bool> viewPermissions = new Dictionary<string, bool>();

            try
            {
                viewPermissions.Add(ViewPermissionsConstants.CanViewAllAdminFunctions, false);
                viewPermissions.Add(ViewPermissionsConstants.CanViewStudentCounts, false);

                if (appRole.Name == "Administrator" || appRole.Name == "EDVP - Admin")
                {
                    viewPermissions[ViewPermissionsConstants.CanViewAllAdminFunctions] = true;
                    viewPermissions[ViewPermissionsConstants.CanViewStudentCounts] = true;
                }
               // _loggingService.LogInfoMessage($"Successfully set permissions for user;");
            }
            catch (Exception ex)
            {
                //_loggingService.LogInfoMessage($"Failed to set permissions for user; See error message: {ex.Message}");
            }
            return viewPermissions;
        }

    }
}



