using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IAppUserService
    {
        AppUser GetCurrentAppUser(int sessionId);
        AppUserSession GetSession(int sessionId);
        int CreateAppUserSession(int appUserId);
        void DismissAnnouncement(int sessionId, int announcementId);
    }
}

