using System.Net.Http;
using SimpleInjector;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public sealed class RequestMessageAccessor : IRequestMessageAccessor
    {
        private readonly Container _container;

        public RequestMessageAccessor(Container container)
        {
            _container = container;
        }

        public HttpRequestMessage CurrentMessage => _container.GetCurrentHttpRequestMessage();
    }
}