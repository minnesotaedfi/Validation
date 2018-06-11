using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeEdOrgService : IEdOrgService
    {
        public List<EdOrg> GetEdOrgs()
        {
            var mde = new EdOrg { Id = "MDE", Name = "MN Department of Education", Parent = null, Type = EdOrgType.State };
            var sRegion = new EdOrg { Id = "South", Name = "Southern Region", Parent = mde, Type = EdOrgType.Region };
            var d622 = new EdOrg { Id = "ISD 622", Name = "North St. Paul-Maplewood School District", Parent = sRegion, Type = EdOrgType.District };
            var d625 = new EdOrg { Id = "ISD 625", Name = "St. Paul School District", Parent = sRegion, Type = EdOrgType.District };
            var s6221 = new EdOrg { Id = "School 622-1", Name = "Eagle Point Elementary School", Parent = d622, Type = EdOrgType.School };
            var s6222 = new EdOrg { Id = "School 622-2", Name = "John Glenn Middle School", Parent = d622, Type = EdOrgType.School };

            return new List<EdOrg> { mde, sRegion, d622, d625, s6221, s6222 };
        }

        public EdOrg GetEdOrgById(string edOrgId)
        {
            return GetEdOrgs().FirstOrDefault(eorg => eorg.Id == edOrgId);
        }
    }
}