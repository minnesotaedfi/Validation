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
            var aSchool = ValidationPortalDbMigrationConfiguration.EdOrgTypeLookups.First(eot => eot.CodeValue == EdOrgType.School.ToString());
            var aDistrict = ValidationPortalDbMigrationConfiguration.EdOrgTypeLookups.First(eot => eot.CodeValue == EdOrgType.District.ToString());
            var aRegion = ValidationPortalDbMigrationConfiguration.EdOrgTypeLookups.First(eot => eot.CodeValue == EdOrgType.Region.ToString());
            var aState = ValidationPortalDbMigrationConfiguration.EdOrgTypeLookups.First(eot => eot.CodeValue == EdOrgType.State.ToString());

            var mde = new EdOrg { Id = "MDE", Name = "MN Department of Education", Parent = null, Type = aState };
            var sRegion = new EdOrg { Id = "South", Name = "Southern Region", Parent = mde, Type = aRegion };
            var d622 = new EdOrg { Id = "ISD 622", Name = "North St. Paul-Maplewood School District", Parent = sRegion, Type = aDistrict };
            var d625 = new EdOrg { Id = "ISD 625", Name = "St. Paul School District", Parent = sRegion, Type = aDistrict };
            var s6221 = new EdOrg { Id = "School 622-1", Name = "Eagle Point Elementary School", Parent = d622, Type = aSchool };
            var s6222 = new EdOrg { Id = "School 622-2", Name = "John Glenn Middle School", Parent = d622, Type = aSchool };

            return new List<EdOrg> { mde, sRegion, d622, d625, s6221, s6222 };
        }

        public EdOrg GetEdOrgById(string edOrgId)
        {
            return GetEdOrgs().FirstOrDefault(eorg => eorg.Id == edOrgId);
        }
    }
}