using System.Web;
using System.Web.Optimization;

namespace socisaWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/AllScripts").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/ng-file-upload-shim.js",
                        "~/Scripts/ng-file-upload.js",
                        "~/Scripts/angular-animate.js",
                        "~/Scripts/ngDialog.js",
                        "~/Scripts/spinner.js",
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/modernizr-*",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/html5shiv.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/SocisaApp.js",
                        "~/Scripts/Controllers/*Controller.js"
                        ));


            bundles.Add(new StyleBundle("~/Content/AllStyles").Include(
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
