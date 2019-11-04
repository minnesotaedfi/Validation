using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

using NUnit.Framework;

using Should;

using ValidationWeb.Models;
using ValidationWeb.Utility;

namespace ValidationWeb.Tests.Utility
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class IIdentityExtensionsTests
    {
        [Test]
        [TestCase(SSORoleNames.DataOwner, TestName = "GetViewPermissions_DataOwner")]
        [TestCase(SSORoleNames.Admin, TestName = "GetViewPermissions_Admin")]
        [TestCase(SSORoleNames.DistrictUser, TestName = "GetViewPermissions_DistrictUser")]
        [TestCase(SSORoleNames.HelpDesk, TestName = "GetViewPermissions_HelpDesk")]
        [TestCase(SSORoleNames.RegionUser, TestName = "GetViewPermissions_RegionUser")]
        public void GetViewPermissions_Should_AssignCorrectRolePermissions(string appRoleName)
        {
            IIdentity identity = new ValidationPortalIdentity();
            var appRole = AppRole.CreateAppRole(appRoleName);
            
            var result = identity.GetViewPermissions(appRole);

            switch (appRoleName)
            {
                case SSORoleNames.DataOwner:
                    result.CanViewStudentDrilldownReports.ShouldBeFalse();
                    result.CanAccessAdminFeatures.ShouldBeFalse();
                    result.CanAccessAllDistrictsMode.ShouldBeTrue();
                    result.CanModifyRecordsRequests.ShouldBeFalse();
                    result.CanRunValidationReports.ShouldBeFalse();
                    result.CanViewOdsReports.ShouldBeTrue();
                    result.CanViewValidationReports.ShouldBeFalse();
                    break;
                case SSORoleNames.Admin:
                    result.CanViewStudentDrilldownReports.ShouldBeTrue();
                    result.CanAccessAdminFeatures.ShouldBeTrue();
                    result.CanAccessAllDistrictsMode.ShouldBeTrue();
                    result.CanModifyRecordsRequests.ShouldBeFalse();
                    result.CanRunValidationReports.ShouldBeFalse();
                    result.CanViewOdsReports.ShouldBeFalse();
                    result.CanViewValidationReports.ShouldBeTrue();
                    break;
                case SSORoleNames.DistrictUser:
                    result.CanViewStudentDrilldownReports.ShouldBeTrue();
                    result.CanAccessAdminFeatures.ShouldBeFalse();
                    result.CanAccessAllDistrictsMode.ShouldBeFalse();
                    result.CanModifyRecordsRequests.ShouldBeTrue();
                    result.CanRunValidationReports.ShouldBeTrue();
                    result.CanViewOdsReports.ShouldBeTrue();
                    result.CanViewValidationReports.ShouldBeTrue();
                    break;
                case SSORoleNames.HelpDesk:
                    result.CanViewStudentDrilldownReports.ShouldBeTrue();
                    result.CanAccessAdminFeatures.ShouldBeFalse();
                    result.CanAccessAllDistrictsMode.ShouldBeTrue();
                    result.CanModifyRecordsRequests.ShouldBeFalse();
                    result.CanRunValidationReports.ShouldBeFalse();
                    result.CanViewOdsReports.ShouldBeTrue();
                    result.CanViewValidationReports.ShouldBeTrue();
                    break;
                case SSORoleNames.RegionUser:
                    result.CanViewStudentDrilldownReports.ShouldBeFalse();
                    result.CanAccessAdminFeatures.ShouldBeFalse();
                    result.CanAccessAllDistrictsMode.ShouldBeFalse();
                    result.CanModifyRecordsRequests.ShouldBeFalse();
                    result.CanRunValidationReports.ShouldBeFalse();
                    result.CanViewOdsReports.ShouldBeFalse();
                    result.CanViewValidationReports.ShouldBeFalse();
                    break;
            }
        }
    }
}
