using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class EdOrg
    {
        public EdOrg Parent { get; set; }
        public EdOrgType Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}