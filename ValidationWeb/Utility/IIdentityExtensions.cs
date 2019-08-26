using System.Security.Principal;

using ValidationWeb.Models;

namespace ValidationWeb.Utility
{
    /// <summary>
    /// Extensions for IIdentity
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IIdentityExtensions
    {
        /// <summary>
        /// Method to extend IIdentity so that it will return the dictionary of permissions
        /// </summary>
        /// <param name="identity"><see cref="IIdentity"/> of a ValidationPortalIdentity</param>
        /// <param name="role"><see cref="AppRole"/> of the User</param>
        /// <returns>Dictionary with permissions and the settings for that user.</returns>
        public static RolePermissions GetViewPermissions(this IIdentity identity, AppRole role) 
        {
            var rolePermissions = new RolePermissions();

            switch (role.Name)
            {
                case PortalRoleNames.Admin:
                    rolePermissions.CanAccessAdminFeatures = true;
                    rolePermissions.CanViewMarssComparisonLink = true;
                    rolePermissions.CanViewStudentLevelReports = true;
                    rolePermissions.CanViewStudentDrilldownReports = true;
                    rolePermissions.CanViewValidationReports = true;
                    rolePermissions.CanAccessAllDistrictsMode = true;
                    break;

                case PortalRoleNames.HelpDesk:
                    rolePermissions.CanViewOdsReports = true;
                    rolePermissions.CanViewStudentDrilldownReports = true;
                    rolePermissions.CanViewValidationReports = true;
                    rolePermissions.CanAccessAllDistrictsMode = true;
                    rolePermissions.CanViewMarssComparisonLink = true;
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
                    rolePermissions.CanViewMarssComparisonLink = true;
                    rolePermissions.CanViewStudentLevelReports = true;
                    break;
            }

            return rolePermissions;
        }
    }
}