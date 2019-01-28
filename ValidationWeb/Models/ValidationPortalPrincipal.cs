namespace ValidationWeb.Filters
{
    using System.Security.Principal;

    public class ValidationPortalPrincipal : IPrincipal
    {
        public ValidationPortalPrincipal(ValidationPortalIdentity ident)
        {
            Identity = ident;
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



