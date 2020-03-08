using System.Web;
using System.Web.Optimization;

namespace Internal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Assets/Scripts/jquery.{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Assets/Scripts/jquery-2.1.4.min.js",//
                "~/Assets/Scripts/bootstrap-3.1.1.min.js",//
                "~/Assets/Scripts/lightbox-plus-jquery.min.js",
                "~/Assets/Scripts/responsiveslides.min.js",
                "~/Assets/Scripts/move-top.js",
                "~/Assets/Scripts/easing.js",
                "~/Assets/Scripts/custom.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Assets/Scripts/jquery.validate*",
                "~/Assets/Scripts/jquery.unobtrusive-ajax.min.js"
                ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));


            //bundles.Add(new StyleBundle("~/bundles/css").IncludeDirectory("~/Assets/Css", "*.css"));
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Assets/Css/style.css",
                "~/Assets/Css/bootstrap.min.css",
                "~/Assets/Css/font-awesome.min.css",
                "~/Assets/Css/lightbox.css",
                "~/Assets/Css/style.css"
                ));
        }
    }
}
