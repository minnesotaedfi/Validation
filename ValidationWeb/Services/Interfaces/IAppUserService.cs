using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IAppUserService
    {
        void DismissAnnouncement(int announcementId);
        AppUserSession GetSession();
    }
}

