using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ComponentSpace.SAML2;

namespace IdentityProvider.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult SingleSignOnService()
        {
            // Receive the authn request from the service provider (SP-initiated SSO).
            string partnerName;

            SAMLIdentityProvider.ReceiveSSO(Request, out partnerName);

            // If the user is logged in at the identity provider, complete SSO immediately.
            // Otherwise have the user login before completing SSO.
            //if (User.Identity.IsAuthenticated)
            //{
            CompleteSingleSignOn();

            return new EmptyResult();
            //}
            //else
            //{
            //    return RedirectToAction("SingleSignOnServiceCompletion");
            //}
        }

        public ActionResult SingleSignOnServiceCompletion()
        {
            CompleteSingleSignOn();

            return new EmptyResult();
        }

        public ActionResult SingleLogoutService()
        {
            // Receive the single logout request or response.
            // If a request is received then single logout is being initiated by a partner service provider.
            // If a response is received then this is in response to single logout having been initiated by the identity provider.
            bool isRequest;
            bool hasCompleted;
            string logoutReason;
            string partnerName;
            string relayState;

            SAMLIdentityProvider.ReceiveSLO(
                Request,
                Response,
                out isRequest,
                out hasCompleted,
                out logoutReason,
                out partnerName,
                out relayState);

            if (isRequest)
            {
                // Logout locally.

                //JC (comentado)
                //HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                //fin

                // Respond to the SP-initiated SLO request indicating successful logout.
                SAMLIdentityProvider.SendSLO(Response, null);
            }
            else
            {
                if (hasCompleted)
                {
                    // IdP-initiated SLO has completed.
                    if (!string.IsNullOrEmpty(relayState) && Url.IsLocalUrl(relayState))
                    {
                        return Redirect(relayState);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            return new EmptyResult();
        }

        private void CompleteSingleSignOn()
        {
            // Get the name of the logged in user.
            var userName = User.Identity.Name;

            // For demonstration purposes, include some claims.
            var attributes = new Dictionary<string, string>();

            //JC (comentado)
            //foreach (var claim in ((ClaimsIdentity)User.Identity).Claims)
            //{
            //    attributes[claim.Type] = claim.Value;
            //}
            //fin

            // The user is logged in at the identity provider.
            // Respond to the authn request by sending a SAML response containing a SAML assertion to the SP.
            SAMLIdentityProvider.SendSSO(Response, userName, attributes);
        }

        // GET: Saml
        public ActionResult Index()
        {
            return View();
        }
    }
}