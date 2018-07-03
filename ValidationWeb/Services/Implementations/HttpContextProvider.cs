using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class HttpContextProvider : IHttpContextProvider
    {
        public HttpContext CurrentHttpContext
        {
            get
            {
                return HttpContext.Current ?? new HttpContext(
                    new HttpRequest("none.html", "https://localhost/none.html", string.Empty),
                    new HttpResponse(new System.IO.StringWriter()));
            }
        }
    }
}
