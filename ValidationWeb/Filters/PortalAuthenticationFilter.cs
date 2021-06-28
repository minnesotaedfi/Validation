﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using MoreLinq;

using SimpleInjector;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Filters
{
    /// <summary>
    /// Register this class using GlobalFilters.Add() in Global.asax.
    /// </summary>
    public class PortalAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        private static readonly object StaticValuesLock = new object();

        private static IConfigurationValues _config;

        /// <summary>
        /// Used to call the stored procedure that obtains detailed information about the user represented by the single sign-on token/header-value.
        /// </summary>
        private static string _authorizationStoredProcedureName;

        /// <summary>
        /// Used to call the stored procedure that obtains detailed information about the user represented by the single sign-on token/header-value.
        /// </summary>
        private static string _singleSignOnDatabaseConnectionString;

        /// <summary>
        /// The ID of this web application as assigned/recognized by the authentication server.
        /// </summary>
        private static string _appId;

        private static string _unauthorizedUrl;

        /// <summary>
        /// Name of the ASP.NET/OWIN-provided Session object within the HttpContext
        /// </summary>
        public const string SessionItemName = "Session";

        /// <summary>
        /// Key property value of the ASP.NET/OWIN-provided HttpContext.Session object for the user's session, if it exists. 
        /// </summary>
        public const string SessionKey = "LoggedInUserSessionKey";

        /// <summary>
        /// Name of the cached object in the ASP.NET/OWIN-provided HttpContext.Session that contains user information that's not specific to the session.
        /// </summary>
        public const string SessionIdentityKey = "LoggedInUserIdentity";

        private readonly IEdOrgService _edOrgService;

        private readonly ILoggingService _loggingService;

        private readonly IDbContextFactory<ValidationPortalDbContext> _dbContextFactory;

        public PortalAuthenticationFilter(Container container)
        {
            if (_config == null)
            {
                lock (StaticValuesLock)
                {
                    // In case someone waited on the lock, avoid initializing twice by doing a second check.
                    if (_config == null)
                    {
                        _config = container.GetInstance<IConfigurationValues>();
                        _authorizationStoredProcedureName = _config.AuthorizationStoredProcedureName;
                        _singleSignOnDatabaseConnectionString = _config.SingleSignOnDatabaseConnectionString;
                        _appId = _config.AppId;
                        _unauthorizedUrl = _config.EdiamUnauthorizedLink;
                    }
                }
            }

            _edOrgService = container.GetInstance<IEdOrgService>();
            _loggingService = container.GetInstance<ILoggingService>();
            _dbContextFactory = container.GetInstance<IDbContextFactory<ValidationPortalDbContext>>();
        }

        protected bool IsAnonymousAction(ActionDescriptor descriptor)
        {
            var action = descriptor
                .GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any();

            var controller = descriptor.ControllerDescriptor
                .GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any();

            return action || controller;
        }

        /// <summary>
        /// Goal: set the HttpContext.User with a System.Security.Principal.IPrincipal
        /// </summary>
        /// <param name="filterContext">The <see cref="AuthenticationContext"/></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Values are read from the config file")]
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (IsAnonymousAction(filterContext.ActionDescriptor))
            {
                return;
            }

            var isAuthenticated = false;

            var httpContext = filterContext.RequestContext.HttpContext;
            var session = httpContext.Session;
            var request = httpContext.Request;

            // We will try to set the user's Focused items to the same ones in their previous, expired session, if it is feasible. 
            int previousSessionFocusedEdOrgId = 0;
            int? previousSessionFocusedSchoolYearId = null;

            var validYears = new List<SchoolYear>();
            using (var dbContext = _dbContextFactory.Create())
            {
                validYears
                    .AddRange(dbContext.SchoolYears
                        .Where(sy => sy.Enabled)
                        .Where(sy => sy.Visible)
                        .OrderBy(x => x.StartYear).ToList());
            }

            if (validYears.Count == 0)
            {
                throw new ApplicationException("No school years were enabled in the Validation Portal's database. Data is separated by school year, and so the system must know which school years are available for users.");
            }

            #region If we find the Session Key checks out, then that means they were previously authenticated. 
            // The session has been configured to use SQL Server state - so load balancers/web farms are fine.
            if (!session.IsNewSession && session[SessionKey] != null)
            {
                // If the Microsoft Session (configured in Web.Config) has timed out - then IsNewSession would have returned true, 
                // and we wouldn't get here.
                // The cached session is checked against the timeout a bit further down, once the code has retrieved the cached Session
                // Recall the user's info from the previously stored session state.
                var userIdentity = session[SessionIdentityKey] as ValidationPortalIdentity;
                if (userIdentity != null)
                {
                    // IMPORTANT - tell ASP.NET we know who the user is - prevents from redirecting the user to login page in a subsequent handler.
                    httpContext.User = new ValidationPortalPrincipal(userIdentity);
                    _loggingService.LogInfoMessage($"MVC request authenticated for user {userIdentity.FullName}.");
                    using (var dbContext = _dbContextFactory.Create())
                    {
                        // Get the user's session from the database.
                        var sessionIdSought = session[SessionKey].ToString();
                        var currentSession = dbContext.AppUserSessions.FirstOrDefault(sess => sess.Id == sessionIdSought);
                        if (currentSession != null)
                        {
                            previousSessionFocusedEdOrgId = currentSession.FocusedEdOrgId;

                            if (validYears.Any(x => x.Id == currentSession.FocusedSchoolYearId))
                            {
                                previousSessionFocusedSchoolYearId = currentSession.FocusedSchoolYearId;
                            }

                            if (currentSession.ExpiresUtc > DateTime.UtcNow)
                            {
                                // Extend the current session.
                                currentSession.ExpiresUtc = currentSession.ExpiresUtc.AddMinutes(30);
                                dbContext.SaveChanges();
                                _loggingService.LogInfoMessage($"Existing session extended for user {userIdentity.FullName}.");

                                // Fill in the user's Identity info on the session instance, which is not persisted in the database.
                                currentSession.UserIdentity = userIdentity;

                                // Make the session accessible throughout the request.
                                httpContext.Items[SessionItemName] = currentSession;

                                // No need to create a new session - so exit the method now.
                                return;
                            }
                            else
                            {
                                // The session has expired from our application's constraint.
                                _loggingService.LogInfoMessage($"Expired session for user {userIdentity.FullName} removed.");
                                dbContext.AppUserSessions.Remove(currentSession);
                                dbContext.SaveChanges();

                                // And some clean-up ...
                                //IEnumerable<AppUserSession> expiredUserSessions = dbContext.AppUserSessions.Where(s => s.UserIdentity == null || (s.UserIdentity.UserId == userIdentity.UserId && s.ExpiresUtc < DateTime.UtcNow));
                                // TODO - IDENTITIES NOT IN EF
                                //dbContext.AppUserSessions.RemoveRange(expiredUserSessions);

                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
                // If the code reaches this point, it means the user needs a new session - otherwise the "return" statement would have been executed.
            }
            #endregion If we find the Session Key checks out, then load the user's session, and skip checking the database for authorizations. 

            #region Since there wasn't a session, we will authenticate. Make sure the HTTP header placed by the Login Page is present. 
            var authHeaderValue = request.Headers["HTTP_OBLIX_UID"];
            if (_config.UseSimulatedSSO)
            {
                var cookie = filterContext.HttpContext.Request.Cookies["MockSSOAuth"];

                if (cookie == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }

                authHeaderValue = cookie.Value;

                _loggingService.LogInfoMessage($"Test mock for Single Sign On is activated - simulated user is {authHeaderValue}");
            }
            else
            {
                _loggingService.LogInfoMessage($"Single Sign On user found in header - authenticated user is {authHeaderValue ?? "null"}");
            }

            if (string.IsNullOrWhiteSpace(authHeaderValue))
            {
                // Returning without an HttpContext.User being set is going to cause an 401 UNAUTHORIZED HTTP response to occur in the OnAuthenticationChallenge handler below.
                return;
            }
            #endregion Since there wasn't a session, we will authenticate. Make sure the HTTP header placed by the Login Page is present. 

            #region If no session, then retrieve user access information from single sign on database.
            _loggingService.LogInfoMessage($"New session for {authHeaderValue}, retrieving authorizations remotely.");
            var ssoUserAuthorizations = new List<SsoUserAuthorization>();
            try
            {
                using (var ssoDatabaseConnection = new SqlConnection(_singleSignOnDatabaseConnectionString))
                {
                    var sqlCommandText = $"select * from {_authorizationStoredProcedureName} where UserId = @UserId and AppId = @AppId";
                    var ssoCommand = new SqlCommand(sqlCommandText, ssoDatabaseConnection)
                    {
                        CommandType = CommandType.Text
                    };

                    var userIdInput = ssoCommand.Parameters.Add("@UserId", SqlDbType.VarChar);
                    userIdInput.Value = authHeaderValue;

                    var appIdInput = ssoCommand.Parameters.Add("@AppId", SqlDbType.VarChar);
                    appIdInput.Value = _appId;

                    ssoDatabaseConnection.Open();
                    var ssoReader = ssoCommand.ExecuteReader();
                    while (ssoReader.Read())
                    {
                        // TODO: make sure this lines up with what's really coming from the new MN SSO view 
                        // var stateOrganizationId = $"{ssoReader["districtType"]}{ssoReader["districtNumber"]:D4}000";
                        var stateOrganizationId = $"{ssoReader["stateOrganizationId"]}";
                        if ((stateOrganizationId.Length == 11 || stateOrganizationId.Length == 12) && stateOrganizationId.EndsWith("000"))
                        {
                            stateOrganizationId = stateOrganizationId.Substring(0, stateOrganizationId.Length - 3);
                        }

                        var theAppId = ssoReader["AppId"]?.ToString();

                        if (string.Compare(theAppId, _config.AppId, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            ssoUserAuthorizations.Add(
                                new SsoUserAuthorization
                                {
                                    AppId = theAppId,
                                    AppName = ssoReader["AppName"]?.ToString(),
                                    DistrictNumber = int.Parse(stateOrganizationId),
                                    Email = ssoReader["Email"]?.ToString(),
                                    FirstName = ssoReader["FirstName"]?.ToString(),
                                    MiddleName = ssoReader["MiddleName"]?.ToString(),
                                    LastName = ssoReader["LastName"]?.ToString(),
                                    FullName = ssoReader["FullName"]?.ToString(),
                                    UserId = ssoReader["UserId"]?.ToString(),
                                    StateOrganizationId = stateOrganizationId,
                                    OrganizationName = ssoReader["OrganizationName"]?.ToString(),
                                    RoleDescription = ssoReader["RoleDescription"]?.ToString(),
                                    RoleId = ssoReader["RoleId"]?.ToString()
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"Error occurred when retrieving authorization for user {authHeaderValue}. {ex.ChainInnerExceptionMessages()}");
            }
            #endregion Retrieve user access from single sign on database

            try
            {
                #region Extract data about the user that is common to all SSO Authorization records.

                _loggingService.LogDebugMessage(
                    $"Extracting authorization information from remote authorization response for {authHeaderValue}.");

                // Filter on App ID
                ssoUserAuthorizations.RemoveAll(ss => string.Compare(ss.AppId, _appId, StringComparison.OrdinalIgnoreCase) != 0);

                _loggingService.LogDebugMessage(
                    $"Found {ssoUserAuthorizations.Count} auth records for app id {_appId}");

                foreach (var ssoUserAuth in ssoUserAuthorizations)
                {
                    _loggingService.LogDebugMessage(ssoUserAuth.ToString());
                }

                // Role - grab the first one if there are more than one. This is okay because above we filter records to the Validation Portal application only.
                var userAuthWithAnyRole = ssoUserAuthorizations.FirstOrDefault(ss => ss.RoleId != null);
                if (userAuthWithAnyRole == null)
                {
                    throw new InvalidOperationException("No user found with any role ID");
                }

                var theRole = userAuthWithAnyRole.RoleId;

                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Role: {theRole}.");
                var appRole = AppRole.CreateAppRole(theRole);

                // Role Description
                var theRoleDescription = ssoUserAuthorizations.FirstOrDefault(ss => ss.RoleDescription != null)
                    .RoleDescription;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Role Description: {theRoleDescription}.");

                // UserId
                var theUserId = ssoUserAuthorizations.FirstOrDefault(ss => ss.UserId != null).UserId;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, UserId: {theUserId}.");

                if (ssoUserAuthorizations.Select(ss1 => ss1.RoleId).Distinct()
                        .Count(rid => !string.IsNullOrWhiteSpace(rid)) > 1)
                {
                    _loggingService.LogWarningMessage(
                        $"The user {theUserId} has been assigned more than one role for the Validation Portal application {_appId}. The role {theRole} was used because it was the first encountered.");
                }

                // Names and Addresses
                var firstName = ssoUserAuthorizations.FirstOrDefault(ss => ss.FirstName != null)?.FirstName;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, First Name: {firstName}.");
                var middleName = ssoUserAuthorizations.FirstOrDefault(ss => ss.MiddleName != null)?.MiddleName;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Middle Name: {middleName}.");
                var lastName = ssoUserAuthorizations.FirstOrDefault(ss => ss.LastName != null)?.LastName;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Last Name: {lastName}.");
                var fullName = ssoUserAuthorizations.FirstOrDefault(ss => ss.FullName != null)?.FullName;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Full Name: {fullName}.");
                var theEmail = ssoUserAuthorizations.FirstOrDefault(ss => ss.Email != null)?.Email;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Email: {theEmail}.");
                var appName = ssoUserAuthorizations.FirstOrDefault(ss => ss.AppName != null)?.AppName;
                _loggingService.LogDebugMessage($"User: {authHeaderValue}, Application Name: {appName}.");

                #endregion Extract data about the user that is common to all SSO Authorization records.

                // A school year is needed to identify which Ed Fi ODS database to pull organizational information from.
                int schoolYearId; 
                if (previousSessionFocusedSchoolYearId != null && 
                    validYears.All(x => x.Id != previousSessionFocusedSchoolYearId))
                {
                    schoolYearId = previousSessionFocusedSchoolYearId.Value;
                }
                else
                {
                    schoolYearId = validYears.First().Id;
                }

                var authorizedEdOrgs = new List<EdOrg>();
                foreach (var ssoUserOrg in ssoUserAuthorizations)
                {
                    try
                    {
                        _loggingService.LogDebugMessage(
                            $"*** User: {authHeaderValue}, Ed Organization ID: {ssoUserOrg?.DistrictNumber}.");

                        if (ssoUserOrg != null && !ssoUserOrg.StateOrganizationId.Equals("ALL", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var edOrg = (int)(ssoUserOrg?.DistrictNumber.Value);
                            _loggingService.LogDebugMessage(
                                $"User: {authHeaderValue}, EdOrg: '{edOrg}'. Taking information from the ODS associated with school year ID number {schoolYearId.ToString()}.");
                            try
                            {
                                authorizedEdOrgs.Add(_edOrgService.GetEdOrgById(edOrg, schoolYearId));
                            }
                            catch (Exception ex)
                            {
                                _loggingService.LogErrorMessage(
                                    $"When retrieving the information for Organization ID {edOrg} from the ODS associated with school year ID {schoolYearId} for user {authHeaderValue}, an error occurred: {ex.ChainInnerExceptionMessages()}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.LogErrorMessage(
                            $"A user was authorized an Ed Org with the StateOrganizationId: {ssoUserOrg.StateOrganizationId}, but this Ed Org doesn't exist in the ODS database for school year {schoolYearId}. Error: {ex.ChainInnerExceptionMessages()}");
                    }
                }

                // special case for users who can view all districts
                // todo: add to the logic in IIdentityExtensions.GetViewPermissions() 
                var allDistrictRoles = new[]
                                       {
                                           PortalRoleNames.HelpDesk,
                                           PortalRoleNames.DataOwner,
                                           PortalRoleNames.Admin
                                       };

                if (allDistrictRoles.Contains(appRole.Name))
                {
                    var allEdOrgs = _edOrgService.GetAllEdOrgs();
                    authorizedEdOrgs.AddRange(allEdOrgs.ExceptBy(authorizedEdOrgs, x => x.Id));
                }

                if (!authorizedEdOrgs.Any())
                {
                    var unauthorizedMessage =
                        $"The user {theUserId} logged in successfully, and accessed the Validation Portal application, but wasn't authorized any access to Educational Organizations according to EDIDMS (single sign on authorizations), thus couldn't use the application ... or it is possible none of the authorized organizations have been loaded from the Ed Fi Operational Datastore to the Validation database.";
                    _loggingService.LogErrorMessage(unauthorizedMessage);
                    throw new UnauthorizedAccessException(unauthorizedMessage);
                }

                if (appRole.Equals(AppRole.Unauthorized) || (appRole == null))
                {
                    // Do not set the HttpContext User property, so the user will be redirected to Login Page
                    _loggingService.LogInfoMessage(
                        $"The user {authHeaderValue} wasn't authorized - no SSO authorizations applied to the application {appName} with a valid role and organization combination.");
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
                _loggingService.LogInfoMessage(
                    $"Successfully retrieved at least one organization authorization for user {authHeaderValue}; now creating a new session.");

                #region Create and add a new user session to the database.

                var firstEdOrg = newUserIdentity.AuthorizedEdOrgs.FirstOrDefault();
                var newCurrentSession = new AppUserSession
                {
                    Id = Guid.NewGuid().ToString(),
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),    // oof this is configurable! TODO
                    FocusedEdOrgId =
                                                (previousSessionFocusedEdOrgId == 0)
                                                    ? firstEdOrg.Id
                                                    : previousSessionFocusedEdOrgId,
                    FocusedSchoolYearId =
                                                previousSessionFocusedSchoolYearId ?? validYears.First().Id,
                    UserIdentity = newUserIdentity
                };

                // Make the session accessible throughout the request.
                httpContext.Items[SessionItemName] = newCurrentSession;

                // Let ASP.NET save these objects in the session state for the next request.
                session[SessionKey] = newCurrentSession.Id;
                session[SessionIdentityKey] = newUserIdentity;
                _loggingService.LogInfoMessage($"User {authHeaderValue}; session {newCurrentSession.Id} created.");
                using (var dbContext = _dbContextFactory.Create())
                {
                    dbContext.AppUserSessions.Add(newCurrentSession);
                    dbContext.SaveChanges();
                }

                _loggingService.LogInfoMessage($"User {authHeaderValue}; session {newCurrentSession.Id} saved.");
                isAuthenticated = true;

                #endregion Create and add a new user session to the database.
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"Error occurred when retrieving authorization for user {authHeaderValue}. {ex.ChainInnerExceptionMessages()}");
            }

            if (!isAuthenticated)
            {
                filterContext.Result = new RedirectResult(_unauthorizedUrl);
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (IsAnonymousAction(filterContext.ActionDescriptor))
            {
                return;
            }

            var user = filterContext.HttpContext.User;
            if (user == null)
            {
                _loggingService.LogInfoMessage($"User unauthenticated. Redirecting to login page {_config.AuthenticationServerRedirectUrl ?? "null"} and return URL {filterContext.HttpContext.Request.Url}.");
                var redirectUrl = $"{_config.AuthenticationServerRedirectUrl}?returnUrl={System.Net.WebUtility.UrlEncode(filterContext.HttpContext.Request.Url?.ToString())}";
                filterContext.Result = new RedirectResult(redirectUrl);
            }
            else if (!user.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(_unauthorizedUrl);
            }
        }
    }
}