using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ValidationAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        /// <summary>
        /// Name of the ASP.NET/OWIN-provided Session object within the HttpContext
        /// </summary>
        public const string SessionItemName = "Session";
        /// <summary>
        /// Key property value of the ASP.NET/OWIN-provided HttpCpntext.Session object for the user's session, if it exists. 
        /// </summary>
        public const string SessionKey = "LoggedInUserSessionKey";
        /// <summary>
        /// Name of the cached object in the ASP.NET/OWIN-provided HttpCpntext.Session that contains user information that's not specific to the session.
        /// </summary>
        public const string SessionIdentityKey = "LoggedInUserIdentity";

        private readonly ILoggingService _logger;
        private readonly HttpConfiguration _globalConfig;

        public ValidationAuthenticationFilter(HttpConfiguration globalConfig, ILoggingService logger)
        {
            _logger = logger;
            // We are going to need some AsyncScoped dependencies, but we are outside of a scope, so resort to Service Locator pattern
            // by using the DependencyResolver in the global configuration. http://simpleinjector.readthedocs.io/en/latest/webapiintegration.html
            _globalConfig = globalConfig;
        }

        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Based on the content of the HTTP Authorization request header.
        /// </summary>
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var httpContext = HttpContext.Current;
            var session = HttpContext.Current?.Session;
            if (session != null)
            {
                if (! HttpContext.Current.Session.IsNewSession)
                {
                    context.Principal = new ValidationPortalPrincipal(new ValidationPortalIdentity());

                    // If the Microsoft Session (configured in Web.Config) has timed out - then IsNewSession would have returned true, and we wouldn't get here.
                    // The cached session is checked against the timeout a bit further down, once the code has retrieved the cached Session
                    // Recall the user's info from the previously stored session state.
                    var userIdentity = session[SessionIdentityKey] as ValidationPortalIdentity;
                    if (userIdentity != null)
                    {
                        // IMPORTANT - tell ASP.NET we know who the user is - prevents from redirecting the user to login page in a subsequent handler.
                        httpContext.User = new ValidationPortalPrincipal(userIdentity);
                        context.Principal = httpContext.User;
                        using (var dbContext = new ValidationPortalDbContext())
                        {
                            // Get the user's session from the database.
                            var sessIdSought = session[SessionKey].ToString();
                            var currentSession = dbContext.AppUserSessions.FirstOrDefault(sess => sess.Id == sessIdSought);
                            if (currentSession != null)
                            {
                                if (currentSession.ExpiresUtc > DateTime.UtcNow)
                                {
                                    // Extend the current session.
                                    currentSession.ExpiresUtc = currentSession.ExpiresUtc.AddMinutes(30);
                                    dbContext.SaveChanges();
                                    // Fill in the user's Identity info on the session instance, which is not persisted in the database.
                                    currentSession.UserIdentity = userIdentity;
                                    // Make the session accessible throughout the request.
                                    httpContext.Items[SessionItemName] = currentSession;
                                }
                                else
                                {
                                    // The session has expired from our application's constraint.
                                    dbContext.AppUserSessions.Remove(currentSession);
                                    dbContext.SaveChanges();
                                    // And some clean-up ...
                                    IEnumerable<AppUserSession> expiredUserSessions = dbContext.AppUserSessions.Where(s => s.ExpiresUtc < DateTime.UtcNow);
                                    dbContext.AppUserSessions.RemoveRange(expiredUserSessions);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            } 
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Basic");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }

    }
}