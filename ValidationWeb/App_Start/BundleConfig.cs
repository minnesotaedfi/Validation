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

            bundles.Add(new ScriptBundle("~/bundles/table2CSV").Include("~/Content/Scripts/table2csv.js"));
            bundles.Add(new ScriptBundle("~/bundles/tabulatorScripts").Include("~/Content/Scripts/tabulator.js"));
            bundles.Add(new StyleBundle("~/Content/tabulatorCSS").Include("~/Content/Styles/tabulator_simple.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include("~/Content/Scripts/jquery-ui.js"));
            bundles.Add(new StyleBundle("~/Content/jqueryUICSS").Include("~/Content/Styles/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/datePickerCSS").Include("~/Content/Styles/bootstrap-datetimepicker.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/datePickerJS").Include("~/Content/Scripts/moment.js", "~/Content/Scripts/bootstrap-datetimepicker.min.js"));
        }
    }
}
