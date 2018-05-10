using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MDE.ValidationPortal
{
    // TODO: Delete This - or modify to suit


    /// <summary>
    /// Code applied to all incoming HTTP requests to authenticate the Client. Automatically chooses between
    /// OAuth versions and hash algorithms based on the content of the HTTP Authorization request header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class CustomAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        private readonly IApplicationLoggerService _logger;
        private readonly HttpConfiguration _globalConfig;
        private readonly string _adminHelpString;
        const int allowedClockDifferenceInSeconds = 180;

        public CustomAuthenticationFilter(HttpConfiguration globalConfig, IApplicationLoggerService logger)
        {
            _logger = logger;
            // We are going to need some AsyncScoped dependencies, but we are outside of a scope, so resort to Service Locator pattern
            // by using the DependencyResolver in the global configuration. http://simpleinjector.readthedocs.io/en/latest/webapiintegration.html
            _globalConfig = globalConfig;
            _adminHelpString = $"Ask administrator for details, Log Correlation ID: { _logger.GetLogCorrelationId()}";
        }

        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

            // TODO: Decide what to do about this overidden method - currently letting everyone through. 
        /// <summary>
        /// Decides which OAuth version the client is using based on the content of the HTTP Authorization request header.
        /// </summary>
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // This is out-of-scope for the Dependency Injection "AsyncScoped dependencies", so use the DI Container as a Service Locator.
            //var authService = _globalConfig.DependencyResolver.GetService(typeof(IAuthService)) as IAuthService;
            //authService.FlushExpiredTokens();
            //ProcessOAuthV2(authService, context, cancellationToken);
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Basic");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Checks Client Credentials to Authenticate OAuth 2 clients.
        /// </summary>
        private void ProcessOAuthV2(/* IAuthService authService, */ HttpAuthenticationContext context, CancellationToken cancellationToken)
        {

            context.Principal = new PortalPrincipal("Anonymous");
            /*
            try
            {
                var authHeader = context?.Request?.Headers?.Authorization;
                if (authHeader == null)
                {
                    throw new Exception("No \"Authorization\" request header present.");
                }
                var scheme = authHeader.Scheme.ToUpper();
                var parameter = authHeader.Parameter;
                #region Providing Client Credentials Directly.
                if (scheme == "BASIC")
                {
                    var textEncoder = Encoding.UTF8;

                    var credentials = textEncoder.GetString(Convert.FromBase64String(parameter.Trim()));
                    var clientKeySecret = credentials.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (clientKeySecret.Length != 2)
                    {
                        var problem0 = $"OAuth 2 Client Credentials Grant Type requires the Authorization HTTP header to use the scheme \"Basic\", with the data formatted: \"client-id:client-secret\". {_adminHelpString}";
                        _logger.LogErrorMessage(problem0);
                        context.ErrorResult = new AuthenticationFailureResult(problem0, context.Request);
                        return;
                    }
                    var clientId = clientKeySecret[0];
                    var clientSecret = clientKeySecret[1];
                    var authenticatedClient = authService.AuthenticateClient(clientId, clientSecret);
                    if (authenticatedClient != null)
                    {
                        context.Principal = new PortalPrincipal(clientId);
                        context.Request.Properties.Add("Client", authenticatedClient);
                        _logger.LogInfoMessage($"Authenticated with Client ID: {clientId}");
                        return;
                    }
                    var problem1 = $"OAuth 2 Client {clientId} authentication failed. Client ID or secret was invalid. {_adminHelpString}";
                    _logger.LogErrorMessage(problem1);
                    context.ErrorResult = new AuthenticationFailureResult(problem1, context.Request);
                    return;
                }
                #endregion Providing Client Credentials Directly.

                #region Providing a Token obtained previously using Client Credentials.
                if (scheme == "BEARER")
                {
                    var authenticatedToken = authService.ValidateBearerToken(parameter);
                    if (authenticatedToken != null)
                    {
                        _logger.LogInfoMessage($"Token validated. Refreshing token: {parameter}");
                        authService.RefreshBearerToken(parameter);
                        context.Principal = new PortalPrincipal(parameter);
                        context.Request.Properties.Add("Token", authenticatedToken);
                    }
                    else
                    {
                        var problem2 = $"OAuth 2 Bearer Token is invalid or expired. {_adminHelpString}";
                        _logger.LogErrorMessage(problem2);
                        context.ErrorResult = new AuthenticationFailureResult(problem2, context.Request);
                    }
                    return;
                }
                #endregion Providing a Token obtained previously using Client Credentials.

            }
            catch (Exception ex)
            {
                var problem3 = $"OAuth 2 Client Credentials Grant Type requires the Authorization HTTP header to use the scheme \"Basic \", with the data formatted: \"client-id:client-secret\". Error: {ex.Message} {_adminHelpString}";
                _logger.LogErrorMessage(problem3);
                context.ErrorResult = new AuthenticationFailureResult(problem3, context.Request);
            }
            */
        }
    }
}

