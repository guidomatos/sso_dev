using System.Web.Mvc;

namespace SSO_UPCI.Areas.Proceso
{
    public class ProcesoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Proceso";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Proceso_default",
                "Proceso/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SSO_UPCI.Areas.Proceso.Controllers" }
            );
        }
    }
}