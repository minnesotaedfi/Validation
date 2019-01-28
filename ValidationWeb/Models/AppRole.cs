using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    using ValidationWeb.Filters;

    /// <summary>
    /// Role information and definitions are stored in the Minnesota SSO database. 
    /// </summary>
    [Serializable]
    public class AppRole
    {
        public static AppRole Unauthorized => CreateAppRole("Unauthorized");

        public static AppRole Administrator => CreateAppRole("Administrator");
        
        public static AppRole HelpDesk => CreateAppRole("HelpDesk");
        
        public static AppRole DataOwner => CreateAppRole("DataOwner");
        
        public static AppRole DistrictUser => CreateAppRole("DistrictUser");
        
        public static AppRole RegionUser => CreateAppRole("RegionUser");
        
        public static AppRole CreateAppRole(string name)
        {
            if (string.CompareOrdinal(name, SSORoleNames.Admin) == 0)
            {
                return new AppRole { Name = "Administrator" };
            }
            if (string.CompareOrdinal(name, SSORoleNames.DataOwner) == 0)
            {
                return new AppRole { Name = "DataOwner" };
            }
            if (string.CompareOrdinal(name, SSORoleNames.DistrictUser) == 0)
            {
                return new AppRole { Name = "DistrictUser" };
            }
            if (string.CompareOrdinal(name, SSORoleNames.RegionUser) == 0)
            {
                return new AppRole { Name = "RegionUser" };
            }
            if (string.CompareOrdinal(name, SSORoleNames.HelpDesk) == 0)
            {
                return new AppRole { Name = "HelpDesk" };
            }

            return new AppRole { Name = "Unauthorized" };
        }
        
        public override bool Equals(object obj)
        {
            return (obj as AppRole)?.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        
        public string Name { get; set; }
    }
}