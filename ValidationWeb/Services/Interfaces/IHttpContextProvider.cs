using System;

using System.Web;

namespace ValidationWeb.Services
{
    public interface IHttpContextProvider
    {
        HttpContext CurrentHttpContext { get; }
    }
}
