using System;
using System.Net.Http;

namespace ValidationWeb.Services
{
    public interface IRequestMessageAccessor
    {
        HttpRequestMessage CurrentMessage { get; }
    }
}
