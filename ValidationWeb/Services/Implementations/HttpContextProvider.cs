using System.Web;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class HttpContextProvider : IHttpContextProvider
    {
        public HttpContext CurrentHttpContext =>
            HttpContext.Current ?? new HttpContext(
                new HttpRequest("none.html", "https://localhost/none.html", string.Empty),
                new HttpResponse(new System.IO.StringWriter()));
    }
}
