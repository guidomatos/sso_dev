using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ComponentSpace.SAML2;
using System.Security.Claims; // tipo claims
using System.IdentityModel.Tokens.Jwt;
using SSO_IdentityServerF;
using SSO_Modelo.Interfaces;
using SSO_SecurityServerF;

namespace SSO_UPCI.Controllers
{
    public class SamlController : Controller
    {
        private readonly IVerifyToken _verifyToken;
        public readonly  IConexion    _conexion;
        private readonly IEncriptador _encriptador;

        public SamlController(IVerifyToken verifyToken, IConexion conexion, IEncriptador encriptador)
        {
            _verifyToken = verifyToken;
            _conexion    = conexion;
            _encriptador = encriptador;
        }

        public ActionResult SingleSignOnService()
        {
            // Recibe la solicitud de autenticación del proveedor de servicios (SSO iniciado por SP).
            string partnerName;

            SAMLIdentityProvider.ReceiveSSO(Request, out partnerName);

            Respuesta _rpta = _conexion.consulta_federada_nom(partnerName);
            Session["codfederada"] = _rpta.CodFederada;
            return RedirectToAction("verificaToken","Saml");

        }
   
        public ActionResult verificaToken()
        {
            HttpCookie cookie = Request.Cookies.Get("tokenSSO");
            if (cookie != null)
            {
                string _token = cookie.Value;
                //Desencripta tripleDES
                //string _stKey = System.Configuration.ConfigurationManager.AppSettings["stKey"].ToString();
                //_token = _encriptador.Desencripta3DES(_token, _stKey);
                //Fin TripleDES
                CompleteSingleSignOn(_token);
                return new EmptyResult();
            }
            else
            {
                var _codFederada = (string)Session["codfederada"];
                return RedirectToAction("Index", "Home", new { _f = _codFederada }); //Regresar aqui: /Saml/verificaToken
            }
        }

        private void CompleteSingleSignOn(string _token)
        {
            //Lee token
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(_token) as JwtSecurityToken;
            Claim[] _claims = tokenS.Claims.ToArray();

            string _usuario_login = tokenS.Claims.First(claim => claim.Type == "IdUPC").Value;

            var attributes = new Dictionary<string, string>();
            foreach (var claim in _claims)
            {
                attributes[claim.Type] = claim.Value;
            }
            // Responde a la solicitud de autenticación enviando una respuesta SAML que contenga una aserción SAML al SP.
            SAMLIdentityProvider.SendSSO(Response, _usuario_login, attributes);
        }

        public ActionResult SingleLogoutService()
        {
            // Recibe la solicitud de cierre de sesión único.
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
                //Aqui destruir el token SSO
                _verifyToken.KillToken();

                // Responde a la solicitud de SLO iniciada por el SP que indica un cierre de sesión exitoso.
                SAMLIdentityProvider.SendSLO(Response, null);
            }
            return new EmptyResult();
        }

        public ActionResult Index()
        {
            return View();
        }


        //public ActionResult verificaToken(string _tk, string _hr)
        //{
        //    if (_tk == null && _hr == null)
        //    {
        //        var _AmbienteSSO = System.Configuration.ConfigurationManager.AppSettings["AmbienteSSO"].ToString();
        //        return RedirectToAction("verificaTokenSSO", "Saml", new { _url_de_retorno = _AmbienteSSO + "/Saml/verificaToken" });
        //    }
        //    if (_tk == "OK")
        //    {
        //        CompleteSingleSignOn();
        //        return new EmptyResult();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //public ActionResult verificaTokenSSO(string _url_de_retorno) 
        //{
        //    var _AmbienteSSO = System.Configuration.ConfigurationManager.AppSettings["AmbienteSSO"].ToString();
        //    return Redirect(_AmbienteSSO + "/home/verifica?_url=" + _url_de_retorno);
        //}

    }
}