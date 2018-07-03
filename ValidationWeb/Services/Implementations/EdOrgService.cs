using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class EdOrgService : IEdOrgService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        protected readonly IAppUserService _appUserService;

        public EdOrgService(ValidationPortalDbContext validationPortalDataContext, IAppUserService appUserService)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _appUserService = appUserService;
        }

        public List<EdOrg> GetEdOrgs()
        {
            return _appUserService.GetSession().UserIdentity.AuthorizedEdOrgs.OrderBy(eo => eo.OrganizationName).ToList();
        }

        public EdOrg GetEdOrgById(string edOrgId)
        {
            return GetEdOrgs().FirstOrDefault(eorg => eorg.Id == edOrgId);
        }
    }
}
