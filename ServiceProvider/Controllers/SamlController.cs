using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ComponentSpace.SAML2;
using System.Web.Configuration;

namespace ServiceProvider.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string returnUrl = null)
        {
            var partnerName = WebConfigurationManager.AppSettings["PartnerName"];

            // inicio de sesión único en el proveedor de identidad (SSO iniciado por SP).           
            // La URL de retorno se recuerda como estado de retransmisión SAML.
            SAMLServiceProvider.InitiateSSO(Response, returnUrl, partnerName);

            return new EmptyResult();
        }
        public ActionResult AssertionConsumerService()
        {
            // Reciba y procese la aserción SAML contenida en la respuesta SAML.
            // La respuesta SAML se recibe como parte de SSO iniciado por IdP.
            bool isInResponseTo;
            string partnerName;
            string authnContext;
            string userName;
            IDictionary<string, string> attributes;
            string relayState;

            SAMLServiceProvider.ReceiveSSO(
                Request,
                out isInResponseTo,
                out partnerName,
                out authnContext,
                out userName,
                out attributes,
                out relayState);

            // Automatically provision the user.
            // If the user doesn't exist locally then create the user.
            // Automatic provisioning is an optional step.
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("A SAML Name ID is expected to be returned by the identity provider.");
            }

            return RedirectToAction("Index", "Home", new {_userName = userName});
        }
        public ActionResult SingleLogoutService()
        {
            // Receive the single logout request or response.
            // If a request is received then single logout is being initiated by the identity provider.
            // If a response is received then this is in response to single logout having been initiated by the service provider.
            bool isRequest;
            string logoutReason;
            string partnerName;
            string relayState;

            SAMLServiceProvider.ReceiveSLO(
                Request,
                out isRequest,
                out logoutReason,
                out partnerName,
                out relayState);

            if (isRequest)
            {
                // Logout locally.
                
                //JC (Comentado)
                //HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                //fin

                // Respond to the IdP-initiated SLO request indicating successful logout.
                SAMLServiceProvider.SendSLO(Response, null);
            }
            else
            {
                // SP-initiated SLO has completed.
                //JC (Comentado)
                //if (!string.IsNullOrEmpty(relayState) && Url.IsLocalUrl(relayState))
                //{
                //    return Redirect(relayState);
                //}
                //fin
                return RedirectToAction("Index", "Home");
            }

            return new EmptyResult();
        }

        // GET: Saml
        public ActionResult Index()
        {
            return View();
        }
    }
}