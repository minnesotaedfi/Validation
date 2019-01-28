using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    using ValidationWeb.Filters;

    public interface IAppUserService
    {
        void DismissAnnouncement(int announcementId);
        AppUserSession GetSession();
        ValidationPortalIdentity GetUser();
        void UpdateFocusedEdOrg(string newFocusedEdOrgId);
        void UpdateFocusedSchoolYear(int newFocusedSchoolYearId);
    }
}

