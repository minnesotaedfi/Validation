using System.Web;
using System.Web.Optimization;

namespace ValidationWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // STYLE
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Styles/bootstrap.css",
                      "~/Content/Styles/validationportal.css"));

            // SCRIPTS
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Content/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Content/Scripts/jquery-{version}.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/Scripts/popper.min.js",
                      "~/Content/Scripts/bootstrap.js",
                      "~/Content/Scripts/respond.js"));
        }
    }
}
