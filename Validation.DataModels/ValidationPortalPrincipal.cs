using System.Security.Principal;

namespace ValidationWeb.Models
{
    public class ValidationPortalPrincipal : IPrincipal
    {
        public ValidationPortalPrincipal(ValidationPortalIdentity ident)
        {
            var identity = ident as IIdentity;
            Identity = identity;
        }

        public IIdentity Identity { get; set; }

        public ValidationPortalIdentity PortalIdentity()
        {
            var portalIdentity = Identity as ValidationPortalIdentity;
            return portalIdentity;
        }

        public bool IsInRole(string role)
        {
            var portalIdentity = Identity as ValidationPortalIdentity;
            if (portalIdentity == null)
            {
                return false;
            }
            
            return string.CompareOrdinal(portalIdentity.AppRole.Name, role) == 0;
        }
    }
}



