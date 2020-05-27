using System.Web.Optimization;

namespace ValidationWeb
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/Fonts/FontAwesomeCSS").Include(
                "~/Content/Fonts/fa-solid.css",
                "~/Content/Styles/fontawesome.css"));

            bundles.Add(new StyleBundle("~/Content/BootstrapCSS").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/ValidationPortal")
                .Include("~/Content/Styles/validationportal.css"));

            // SCRIPTS
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/Scripts/jquery-{version}.js",
                "~/Scripts/url-search-params-polyfill.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.js",
                      "~/Scripts/umd/popper-utils,js",
                      "~/Scripts/bootstrap.js",
                      "~/Content/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/table2CSV").Include("~/Content/Scripts/table2csv.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include("~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new StyleBundle("~/Content/jqueryUICSS").Include("~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/datePickerCSS").Include("~/Content/Styles/bootstrap-datetimepicker.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/datePickerJS").Include("~/Content/Scripts/moment.js", "~/Content/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new StyleBundle("~/Content/DataTables/css/DataTablesCSS").Include(
                "~/Content/DataTables/css/dataTables.bootstrap4.css"));

            bundles.Add(
                new StyleBundle("~/Content/Styles/Select2CSS")
                    .Include("~/Content/Styles/select2.css")
                    .Include("~/Content/Styles/select2-bootstrap4.css"));

            bundles.Add(
                new ScriptBundle("~/bundles/Select2")
                    .Include("~/Scripts/select2.full.js"));  // downloaded separately from nuget package

            bundles.Add(
                new StyleBundle("~/Content/Styles/DualListBoxCSS")
                    .Include("~/Content/Styles/bootstrap-duallistbox.css"));

            bundles.Add(
                new ScriptBundle("~/bundles/DualListBox")
                    .Include("~/Scripts/jquery.bootstrap-duallistbox.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/DataTables/dataTables.bootstrap4.js",
                "~/Scripts/DataTables/dataTables.fixedheader.js",
                "~/Scripts/DataTables/dataTables.scroller.js",
                "~/Scripts/DataTables/dataTables.jumpToData.js"));
        }
    }
}
