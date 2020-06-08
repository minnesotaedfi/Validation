using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IAppUserService
    {
        AppUserSession GetSession();
        
        ValidationPortalIdentity GetUser();
        
        void UpdateFocusedEdOrg(string newFocusedEdOrgId);
        
        void UpdateFocusedSchoolYear(int newFocusedSchoolYearId);

        void UpdateFocusedProgramArea(int newFocusedProgramAreaId);
    }
}

