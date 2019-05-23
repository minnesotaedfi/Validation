using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using ValidationWeb.Database;
using ValidationWeb.Services;

namespace ValidationWeb.Filters
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
        /// Name of the cached object in the ASP.NET/OWIN-provided HttpContext.Session that contains user information that's not specific to the session.
        /// </summary>
        public const string SessionIdentityKey = "LoggedInUserIdentity";

        public ValidationAuthenticationFilter()
        {
            var resolver = GlobalConfiguration.Configuration.DependencyResolver;

            Logger = resolver.GetService(typeof(ILoggingService)) as ILoggingService;
            Config = resolver.GetService(typeof(IConfigurationValues)) as IConfigurationValues;
            DbContextFactory =
                resolver.GetService(typeof(IDbContextFactory<ValidationPortalDbContext>)) as
                    IDbContextFactory<ValidationPortalDbContext>;
        }

        public ILoggingService Logger { get; set; }

        public IConfigurationValues Config { get; set; }
        
        public IDbContextFactory<ValidationPortalDbContext> DbContextFactory { get; set; }
        
        public bool AllowMultiple => false;

        /// <summary>
        /// Based on the content of the HTTP Authorization request header.
        /// </summary>
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var httpContext = HttpContext.Current;
            var session = HttpContext.Current?.Session;
            if (session != null)
            {
                if (!HttpContext.Current.Session.IsNewSession)
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
                        using (var dbContext = DbContextFactory.Create())
                        {
                            // Get the user's session from the database.
                            var sessIdSought = session[SessionKey].ToString();
                            var currentSession = dbContext.AppUserSessions.FirstOrDefault(sess => sess.Id == sessIdSought);
                            if (currentSession != null)
                            {
                                Logger.LogInfoMessage($"User {userIdentity.FullName} making a request on an existing session.");
                                if (currentSession.ExpiresUtc > DateTime.UtcNow)
                                {
                                    // Extend the current session.
                                    currentSession.ExpiresUtc = currentSession.ExpiresUtc.AddMinutes(Config.SessionTimeoutInMinutes);
                                    dbContext.SaveChanges();
                                    // Fill in the user's Identity info on the session instance, which is not persisted in the database.
                                    currentSession.UserIdentity = userIdentity;
                                    // Make the session accessible throughout the request.
                                    httpContext.Items[SessionItemName] = currentSession;
                                }
                                else
                                {
                                    Logger.LogInfoMessage($"User {userIdentity.FullName} last session was expired and removed from the database during an API call.");
                                    // The session has expired from our application's constraint.
                                    dbContext.AppUserSessions.Remove(currentSession);
                                    dbContext.SaveChanges();
                                    // And some clean-up ...
                                    IEnumerable<AppUserSession> expiredUserSessions = dbContext.AppUserSessions.Where(s => s.ExpiresUtc < DateTime.UtcNow);
                                    dbContext.AppUserSessions.RemoveRange(expiredUserSessions);
                                    dbContext.SaveChanges();
                                    throw new ApplicationException("Session timeout detected - user must refresh web page to reauthenticate first.");
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