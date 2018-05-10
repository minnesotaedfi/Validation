using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace MDE.ValidationPortal
{
    public sealed class RequestMessageAccessor : IRequestMessageAccessor
    {
        private readonly Container container;

        public RequestMessageAccessor(Container container)
        {
            this.container = container;
        }

        public HttpRequestMessage CurrentMessage =>
            this.container.GetCurrentHttpRequestMessage();
    }
}