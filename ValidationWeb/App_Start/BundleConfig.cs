using System.Web;
using System.Web.Optimization;

namespace ValidationWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Content/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/popper").Include("~/Content/Scripts/popper.min.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Content/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/Scripts/bootstrapjs",
                      "~/Content/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Styles/bootstrap.css",
                      "~/Content/Styles/validationportal.css"));
        }
    }
}
