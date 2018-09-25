using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{
    /// <summary>
    /// Register this class using GlobalFilters.Add() in Global.asax.
    /// </summary>
    public class PortalAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        private static readonly AppSettingsFileConfigurationValues _config;
        /// <summary>
        /// Used to call the stored procedure that obtains detailed information about the user represented by the single sign-on token/header-value.
        /// </summary>
        private static readonly string _authorizationStoredProcedureName;
        /// <summary>
        /// Used to call the stored procedure that obtains detailed information about the user represented by the single sign-on token/header-value.
        /// </summary>
        private static readonly string _singleSignOnDatabaseConnectionString;
        /// <summary>
        /// The ID of this web application as assigned/recognized by the authentication server.
        /// </summary>
        private static readonly string _appId;
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


        static PortalAuthenticationFilter()
        {
            _config = new AppSettingsFileConfigurationValues();
            _authorizationStoredProcedureName = _config.AuthorizationStoredProcedureName;
            _singleSignOnDatabaseConnectionString = _config.SingleSignOnDatabaseConnectionString;
            _appId = _config.AppId;
        }

        /// <summary>
        /// Goal: set the HttpContext.User with a System.Security.Principal.IPrincipal
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var httpContext = filterContext.RequestContext.HttpContext;
            var session = httpContext.Session;
            var request = httpContext.Request;
            // We will try to set the user's Focused items to the same one's in their previous, expired session, if it is feasible. favorite new session variables to 
            string previousSessionFocusedEdOrgId = null;
            int? previousSessionFocusedSchoolYearId = null;

            #region If we find the Session Key checks out, then that means they were previously authenticated. 
            // The session has been configured to use SQL Server state - so load balancers/web farms are fine.
            if (!session.IsNewSession && session[SessionKey] != null)
            {
                // If the Microsoft Session (configured in Web.Config) has timed out - then IsNewSession would have returned true, and we wouldn't get here.
                // The cached session is checked against the timeout a bit further down, once the code has retrieved the cached Session
                // Recall the user's info from the previously stored session state.
                var userIdentity = session[SessionIdentityKey] as ValidationPortalIdentity;
                if (userIdentity != null)
                {
                    // IMPORTANT - tell ASP.NET we know who the user is - prevents from redirecting the user to login page in a subsequent handler.
                    httpContext.User = new ValidationPortalPrincipal(userIdentity);
                    using (var dbContext = new ValidationPortalDbContext())
                    {
                        // Get the user's session from the database.
                        var sessIdSought = session[SessionKey].ToString();
                        var currentSession = dbContext.AppUserSessions.FirstOrDefault(sess => sess.Id == sessIdSought);
                        if (currentSession != null)
                        {
                            previousSessionFocusedEdOrgId = currentSession.FocusedEdOrgId;
                            previousSessionFocusedSchoolYearId = currentSession.FocusedSchoolYearId;
                            if (currentSession.ExpiresUtc > DateTime.UtcNow)
                            {
                                // Extend the current session.
                                currentSession.ExpiresUtc = currentSession.ExpiresUtc.AddMinutes(30);
                                dbContext.SaveChanges();
                                // Fill in the user's Identity info on the session instance, which is not persisted in the database.
                                currentSession.UserIdentity = userIdentity;
                                // Make the session accessible throughout the request.
                                httpContext.Items[SessionItemName] = currentSession;
                                return;
                            }
                            else
                            {
                                // The session has expired from our application's constraint.
                                dbContext.AppUserSessions.Remove(currentSession);
                                dbContext.SaveChanges();
                                // And some clean-up ...
                                IEnumerable<AppUserSession> expiredUserSessions = dbContext.AppUserSessions.Where(s => s.UserIdentity == null || (s.UserIdentity.UserId == userIdentity.UserId && s.ExpiresUtc < DateTime.UtcNow));
                                dbContext.AppUserSessions.RemoveRange(expiredUserSessions);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
                // If the code reaches this point, it means the user needs a new session - otherwise the "return" statement would have been executed.
            }
            #endregion If we find the Session Key checks out, then load the user's session, and skip checking the database for authorizations. 

            #region Since there wasn't a session, we will authenticate. Make sure the HTTP header placed by the Login Page is present. 
            var authHeaderValue = request.Headers["Authorization"];
#if DEBUG
            // TODO: Remove this line
            authHeaderValue = "jane";
#endif
            if (string.IsNullOrWhiteSpace(authHeaderValue))
            {
                // Returning without an HttpContext.User being set is going to cause an 401 UNAUTHORIZED HTTP response to occur in the OnAuthenticationChallenge handler below.
                return;
            }
            #endregion Since there wasn't a session, we will authenticate. Make sure the HTTP header placed by the Login Page is present. 

            #region If no session, then Retrieve user access information from single sign on database.
            var ssoUserAuthorizations = new List<SsoUserAuthorization>();
            using (var ssoDatabaseConnection = new SqlConnection(_singleSignOnDatabaseConnectionString))
            {
                var ssoCommand = new SqlCommand(_authorizationStoredProcedureName, ssoDatabaseConnection);
                ssoCommand.CommandType = CommandType.StoredProcedure;
                var userIdInput = ssoCommand.Parameters.Add("@UserId", SqlDbType.VarChar);
                userIdInput.Value = authHeaderValue;
                ssoDatabaseConnection.Open();
                var ssoReader = ssoCommand.ExecuteReader();
                while (ssoReader.Read())
                {
                    int districtNumber, districtType;
                    var hasDistrictNumber = int.TryParse(ssoReader["DistrictNumber"]?.ToString(), out districtNumber);
                    var hasDistrictType = int.TryParse(ssoReader["DistrictType"]?.ToString(), out districtType);
                    ssoUserAuthorizations.Add(
                        new SsoUserAuthorization
                        {
                            AppId = ssoReader["AppId"]?.ToString(),
                            AppName = ssoReader["AppName"]?.ToString(),
                            DistrictNumber = hasDistrictNumber ? (int?)districtNumber : null,
                            DistrictType = hasDistrictType ? (int?)districtType : null,
                            Email = ssoReader["Email"]?.ToString(),
                            FirstName = ssoReader["FirstName"]?.ToString(),
                            MiddleName = ssoReader["MiddleName"]?.ToString(),
                            LastName = ssoReader["LastName"]?.ToString(),
                            FullName = ssoReader["FullName"]?.ToString(),
                            UserId = ssoReader["UserId"]?.ToString(),
                            StateOrganizationId = ssoReader["StateOrganizationId"]?.ToString(),
                            FormattedOrganizationId = ssoReader["FormattedOrganizationId"]?.ToString(),
                            OrganizationName = ssoReader["OrganizationName"]?.ToString(),
                            RoleDescription = ssoReader["RoleDescription"]?.ToString(),
                            RoleId = ssoReader["RoleId"]?.ToString()
                        });
                }
                ssoReader.Close();
                ssoDatabaseConnection.Close();
            }
            #endregion Retrieve user access from single sign on database

            #region Extract data about the user that is common to all SSO Authorization records.
            // Filter on App ID
            ssoUserAuthorizations.RemoveAll(ss => string.Compare(ss.AppId, _appId, true) != 0);
            // Role - grab the first one if there are more than one.
            var theRole = ssoUserAuthorizations.FirstOrDefault(ss => ss.RoleId != null).RoleId;
            var appRole = AppRole.CreateAppRole(theRole);
            // Role Description
            var theRoleDescription = ssoUserAuthorizations.FirstOrDefault(ss => ss.RoleDescription != null).RoleDescription;
            // UserId
            var theUserId = ssoUserAuthorizations.FirstOrDefault(ss => ss.UserId != null).UserId;
            // Names and Addresses
            var firstName = ssoUserAuthorizations.FirstOrDefault(ss => ss.FirstName != null).FirstName;
            var middleName = ssoUserAuthorizations.FirstOrDefault(ss => ss.MiddleName != null).MiddleName;
            var lastName = ssoUserAuthorizations.FirstOrDefault(ss => ss.LastName != null).LastName;
            var fullName = ssoUserAuthorizations.FirstOrDefault(ss => ss.FullName != null).FullName;
            var theEmail = ssoUserAuthorizations.FirstOrDefault(ss => ss.Email != null).Email;
            var appName = ssoUserAuthorizations.FirstOrDefault(ss => ss.AppName != null).AppName;
            #endregion Extract data about the user that is common to all SSO Authorization records.

            var authorizedEdOrgs = ssoUserAuthorizations.Select(ss => new EdOrg
            {
                // TODO: Align these properties with the Ed Fi "Raw" (Unvalidated) ODS Database
                Id = ss.StateOrganizationId,
                OrganizationName = ss.OrganizationName,
                EdOrgTypeLookupId = (ss.DistrictType.HasValue) ? ValidationPortalDbMigrationConfiguration.EdOrgTypeLookups[ss.DistrictType.Value].Id : 1,
                // TODO: Align these properties with the Single Sign On data
                StateOrganizationId = ss.StateOrganizationId,
                FormattedOrganizationId = ss.FormattedOrganizationId,
                DistrictName = ss.OrganizationName,
                Type = (ss.DistrictType.HasValue) ? ValidationPortalDbMigrationConfiguration.EdOrgTypeLookups[ss.DistrictType.Value] : new EdOrgTypeLookup { Id = 1, CodeValue = "EdOrgTypeLookups", Description = "EdOrgTypeLookups" }
            }).ToList();

            if ((appRole == AppRole.Unauthorized) || (appRole == null))
            {
                // Do not set the HttpContext User property, so the user will be redirected to Login Page
                return;
            }

            // Success - now store the authenticated Principal and create a new session.
            var newUserIdentity = new ValidationPortalIdentity
            {
                AppRole = appRole,
                AuthorizedEdOrgs = authorizedEdOrgs,
                Email = theEmail,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                FullName = fullName,
                Name = theUserId,
                UserId = theUserId
            };
            filterContext.HttpContext.User = new ValidationPortalPrincipal(newUserIdentity);

            var validYears = new List<SchoolYear>();
            using (var dbContext = new ValidationPortalDbContext())
            {
                validYears.AddRange(dbContext.SchoolYears.Where(sy => sy.Enabled).ToList());
            }
            if (validYears.Count == 0)
            {
                throw new ApplicationException("No school years were enabled in the Validation Portal's database.");
            }

            #region Create and add a new user session to the database.
            var firstEdOrg = newUserIdentity.AuthorizedEdOrgs.FirstOrDefault();
            var firstSchoolYear = newUserIdentity.AuthorizedEdOrgs.FirstOrDefault();
            var newCurrentSession = new AppUserSession
            {
                Id = Guid.NewGuid().ToString(),
                DismissedAnnouncements = new HashSet<DismissedAnnouncement>(),
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                FocusedEdOrgId = (previousSessionFocusedEdOrgId == null) ? firstEdOrg.Id : previousSessionFocusedEdOrgId,
                FocusedSchoolYearId = previousSessionFocusedSchoolYearId.HasValue ? previousSessionFocusedSchoolYearId.Value : validYears.First().Id,
                UserIdentity = newUserIdentity
            };
            // Make the session accessible throughout the request.
            httpContext.Items[SessionItemName] = newCurrentSession;
            // Let ASP.NET save these objects in the session state for the next request.
            session[SessionKey] = newCurrentSession.Id;
            session[SessionIdentityKey] = newUserIdentity;
            using (var dbContext = new ValidationPortalDbContext())
            {
                dbContext.AppUserSessions.Add(newCurrentSession);
                dbContext.SaveChanges();
            }
            #endregion Create and add a new user session to the database.
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                var redirectUrl = $"{_config.AuthenticationServerRedirectUrl}?returnUrl={System.Net.WebUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString())}";
                filterContext.Result = new RedirectResult(redirectUrl);
            }
        }
    }
}