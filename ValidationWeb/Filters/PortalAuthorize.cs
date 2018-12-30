using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ValidationWeb.Filters
{
    public class PortalAuthorize : System.Web.Mvc.AuthorizeAttribute
    {
        #if TODO
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                if (filterContext.HttpContext.User.IsInRole("Administrator")) {
                    filterContext.Result = new RedirectResult("~/Admin/Index");
                }
                else {
                    // Otherwise redirect to your specific authorized area
                    filterContext.Result = new RedirectResult("~/Home/Index");
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