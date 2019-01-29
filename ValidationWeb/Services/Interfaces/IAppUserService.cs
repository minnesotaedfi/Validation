namespace ValidationWeb.Services
{
    using ValidationWeb.Filters;

    public interface IAppUserService
    {
        AppUserSession GetSession();
        
        ValidationPortalIdentity GetUser();
        
        void UpdateFocusedEdOrg(string newFocusedEdOrgId);
        
        void UpdateFocusedSchoolYear(int newFocusedSchoolYearId);
    }
}

