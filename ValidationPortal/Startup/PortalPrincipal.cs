using System.Security.Principal;

namespace MDE.ValidationPortal
{
    /// <summary>
    /// A representation of the authenticated, registered client, for use in HTTP Contexts.
    /// </summary>
    public class PortalPrincipal : IPrincipal
    {
        public PortalPrincipal(string name)
        {
            Identity = new PortalIdentity(name);
        }
        public IIdentity Identity { get; set; }
        public bool IsInRole(string role)
        {
            return false;
        }
    }

    /// <summary>
    /// A representation of the authenticated registered client, accessible from HTTP Contexts.
    /// </summary>
    public class PortalIdentity : IIdentity
    {
        public PortalIdentity(string name)
        {
            Name = name;
        }
        public string AuthenticationType { get { return "OAuth"; } }
        public bool IsAuthenticated { get { return true; } }
        public string Name { get; set; }
    }
}
