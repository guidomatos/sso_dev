using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ComponentSpace.SAML2;

namespace ServiceProvider.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string _userName)
        {
            ViewBag.userName = _userName;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
       
        public ActionResult LogOff()
        {

            if (SAMLServiceProvider.CanSLO())
            {
                // Request logout at the identity provider.
                SAMLServiceProvider.InitiateSLO(Response, null, null);

                return new EmptyResult();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}