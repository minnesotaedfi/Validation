using System;
using System.Net.Http;

namespace MDE.ValidationPortal
{
    public interface IRequestMessageAccessor
    {
        HttpRequestMessage CurrentMessage { get; }
    }
}
