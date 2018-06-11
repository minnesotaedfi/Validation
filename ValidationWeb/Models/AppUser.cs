using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EdOrg> AuthorizedEdOrgs { get; set; }
    }
}