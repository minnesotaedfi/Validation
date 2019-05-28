using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Controllers
{
    public class LoginController : Controller
    {
        public LoginController(IConfigurationValues configurationValues)
        {
            ConfigurationValues = configurationValues;
        }

        protected IConfigurationValues ConfigurationValues { get; set; }

        // GET: Login
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            // quick and dirty, don't ship this 
            var results = new List<LoginUser>();

            using (var ssoDatabaseConnection = new SqlConnection(ConfigurationValues.SingleSignOnDatabaseConnectionString))
            {
                var commandText =
                    @"select distinct au.UserId, au.FullName, aa.RoleId, ar.RoleDescription 
                        from apps.AppUser au
                        join apps.AppAuthorization aa on aa.UserId = au.UserId
                        join apps.AppRole ar on ar.RoleId=aa.RoleId";

                using (var ssoCommand = new SqlCommand(commandText, ssoDatabaseConnection))
                {
                    ssoCommand.CommandType = CommandType.Text;
                    ssoDatabaseConnection.Open();
                    var ssoReader = ssoCommand.ExecuteReader();
                    while (ssoReader.Read())
                    {
                        results.Add(
                                new LoginUser
                                {
                                    FullName = ssoReader["FullName"]?.ToString(),
                                    UserId = ssoReader["UserId"]?.ToString(),
                                    RoleDescription = ssoReader["RoleDescription"]?.ToString(),
                                    RoleId = ssoReader["RoleId"]?.ToString()
                                });
                    }
                }
            }

            var viewModel = new LoginViewModel
                            {
                                ReturnUrl = new Uri(returnUrl),
                                LoginUsers = results
                            };

            return View(viewModel);
        }

        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Logout()
        {
            Session.Clear();
            Response.Cookies.Clear();

            HttpCookie cookie = new HttpCookie("MockSSOAuth", string.Empty)
                                {
                                    HttpOnly = true,
                                    Expires = DateTime.Now.AddDays(-1)
                                };

            HttpContext.Response.Cookies.Add(cookie);

            return RedirectToAction(
                "Index", 
                "Login", 
                new
                {
                    returnUrl = Url.Action(
                        "Index", 
                        "Home", 
                        new { }, 
                        Request.Url?.Scheme)
                });
        }

        [AllowAnonymous]
        public ActionResult LoginAsUser(string userId, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (returnUrl == null)
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }

            HttpCookie cookie = new HttpCookie("MockSSOAuth", userId)
                                {
                                    HttpOnly = true,
                                    Expires = DateTime.Now.AddMinutes(10)
                                };

            HttpContext.Response.Cookies.Add(cookie);

            return RedirectToAction(
                "LoginRedirect",
                new
                {
                    returnUrl
                });
        }

        [AllowAnonymous]
        public ActionResult LoginRedirect(string returnUrl)
        {
            return View(new Uri(returnUrl));
        }
    }

    public class LoginViewModel
    {
        public List<LoginUser> LoginUsers { get; set; }

        public Uri ReturnUrl { get; set; }
    }

    public class LoginUser
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string RoleId { get; set; }

        public string RoleDescription { get; set; }
    }
}