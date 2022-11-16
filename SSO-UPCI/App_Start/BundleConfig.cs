using System.Web;
using System.Web.Optimization;

namespace SSO_UPCI
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/style").Include(
                      "~/Content/style.css"));

            bundles.Add(new ScriptBundle("~/bundles/index").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/facebook-script.js",
                "~/Scripts/google-script.js",
                "~/Scripts/constants.js",
                "~/Scripts/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/login.js"));

            bundles.Add(new ScriptBundle("~/bundles/vinculate_accounts").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/facebook-script.js",
                "~/Scripts/google-script.js",
                "~/Scripts/constants.js",
                "~/Scripts/vinculate.js"));

            bundles.Add(new ScriptBundle("~/bundles/recover_pass").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/recover-pass.js"));

            bundles.Add(new ScriptBundle("~/bundles/recover_method").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/recover-method.js"));

            bundles.Add(new ScriptBundle("~/bundles/recover_confirm_sms").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/recover-confirm.js"));

            bundles.Add(new ScriptBundle("~/bundles/recover_confirm_email").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/recover-confirm.js"));

            bundles.Add(new ScriptBundle("~/bundles/recover_verification").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/recover-verification.js"));

            bundles.Add(new ScriptBundle("~/bundles/create_pass").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js",
                "~/Scripts/create-pass.js"));

            bundles.Add(new ScriptBundle("~/bundles/updated_pass").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/Scripts/constants.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
