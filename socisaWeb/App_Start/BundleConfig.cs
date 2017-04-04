using System.Web;
using System.Web.Optimization;

namespace socisaWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Number format
            bundles.Add(new ScriptBundle("~/bundles/hashtable").Include(
                        "~/Scripts/hashtable.js"));
            bundles.Add(new ScriptBundle("~/bundles/hashset").Include(
                        "~/Scripts/hashset.js"));
            bundles.Add(new ScriptBundle("~/bundles/numberformat").Include(
                        "~/Scripts/jquery.numberformatter-1.2.4.js"));

            // Angular scripts
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular.js"));

            // JQuery scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
