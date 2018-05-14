using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MDE.ValidationPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "ValidationReports",
            //    url: "Validation/{action}",
            //    defaults: new { controller = "Validation", action = "Reports", id = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "RulesCollections",
            //    url: "Rules/{*id}",
            //    defaults: new { controller = "Rules", action = "Collections", id = UrlParameter.Optional }
            //);

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
