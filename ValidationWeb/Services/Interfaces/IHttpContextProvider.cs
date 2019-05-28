using System.Web;

namespace ValidationWeb.Services.Interfaces
{
    public interface IHttpContextProvider
    {
        HttpContext CurrentHttpContext { get; }
    }
}
