using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IEdOrgService
    {
        List<EdOrg> GetEdOrgs();
        EdOrg GetEdOrgById(string edorgId);
    }
}
