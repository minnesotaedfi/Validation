using System.Collections.Generic;
using ValidationWeb.Database.Queries;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IEdOrgService
    {
        List<EdOrg> GetAuthorizedEdOrgs();
        
        List<EdOrg> GetAllEdOrgs();
        
        EdOrg GetEdOrgById(int edOrgId, int fourDigitOdsDbYear);

        void RefreshEdOrgCache(SchoolYear schoolYear);

        SingleEdOrgByIdQuery GetSingleEdOrg(int edOrgId, int schoolYearId);
    }
}
