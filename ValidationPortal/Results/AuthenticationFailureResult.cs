using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MDE.ValidationPortal
{
    /// <summary>
    /// Boilerplate code provided by Microsoft - used to send a 401 Unauthorized response when Authentication has failed.
    /// </summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        /// <summary>
        /// Boilerplate code provided by Microsoft - used to send a 401 Unauthorized response when Authentication has failed.
        /// </summary>
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        /// <summary>
        /// Boilerplate code provided by Microsoft - used to send a 401 Unauthorized response when Authentication has failed.
        /// </summary>
        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        /// <summary>
        /// Boilerplate code provided by Microsoft - used to send a 401 Unauthorized response when Authentication has failed.
        /// </summary>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        /// <summary>
        /// Boilerplate code provided by Microsoft - used to send a 401 Unauthorized response when Authentication has failed.
        /// </summary>
        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}
