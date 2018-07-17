using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ValidationPortalAuthServer.Startup))]
namespace ValidationPortalAuthServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
