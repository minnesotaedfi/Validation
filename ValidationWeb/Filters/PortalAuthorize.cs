using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ValidationWeb.Filters
{
    using System.Web.Routing;

    public class PortalAuthorize : System.Web.Mvc.AuthorizeAttribute
    {
#if true
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                if (filterContext.HttpContext.User.IsInRole("Administrator") 
                    && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Admin")
                {

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Admin", action = "Index" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
            }
        }
#else
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
        }
#endif
    }
}