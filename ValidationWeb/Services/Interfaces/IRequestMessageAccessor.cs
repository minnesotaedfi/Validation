using System.Net.Http;

namespace ValidationWeb.Services.Interfaces
{
    public interface IRequestMessageAccessor
    {
        HttpRequestMessage CurrentMessage { get; }
    }
}
