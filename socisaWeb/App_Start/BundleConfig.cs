using System.Web;
using System.Web.Optimization;

namespace socisaWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Angular scripts
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular.js"));

            //Angular file upload
            bundles.Add(new ScriptBundle("~/bundles/ngfileuploadshim").Include(
                        "~/Scripts/ng-file-upload-shim.js"));
            bundles.Add(new ScriptBundle("~/bundles/ngfileupload").Include(
                        "~/Scripts/ng-file-upload.js"));

            //Angular animate
            bundles.Add(new ScriptBundle("~/bundles/angularanimate").Include(
                        "~/Scripts/angular-animate.js"));

            //Angular dialog
            bundles.Add(new ScriptBundle("~/bundles/angulardialog").Include(
                        "~/Scripts/ngDialog.js"));

            //Spinner script
            bundles.Add(new ScriptBundle("~/bundles/spinner").Include(
                        "~/Scripts/spinner.js"));

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
                      "~/Scripts/html5shiv.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/controllers").Include(
                      "~/Scripts/Controllers/*Controller.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
