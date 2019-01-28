namespace ValidationWeb.Utility
{
    using System.Security.Principal;

    using ValidationWeb.Filters;

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