using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Clases entidades
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;
//clases comun
using SSO_UPCI.Clases;
using SSO_SecurityServerF.Mailer;
//interfaces
using SSO_BusinessLogic.Interfaces;
using SSO_IdentityServerF;
using SSO_Modelo.Interfaces;


namespace SSO_UPCI.Areas.Proceso.Controllers
{
    public class HomeController : Controller
    {
        public readonly IConexion _conexion;
        private readonly IVerifyToken _verifyToken;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IProcessLogic _processLogic;
        private readonly IEncriptador _encriptador;
     
        public HomeController(IVerifyToken verifyToken, IConexion conexion, ITokenGenerator tokenGenerator, IProcessLogic processLogic, IEncriptador encriptador)
        {
            _verifyToken = verifyToken;
            _conexion = conexion;
            _tokenGenerator = tokenGenerator;
            _processLogic = processLogic;
            _encriptador = encriptador;
        }
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        public JsonResult CodeSend(Login _login)
        {
            _login.user                   = _encriptador.DecodeBase64(_login.user);                   //decodifica
            _login.usuario_nombre         = _encriptador.DecodeBase64(_login.usuario_nombre);         //decodifica
            _login.Usuario_correoPersonal = _encriptador.DecodeBase64(_login.Usuario_correoPersonal); //decodifica

            if (string.IsNullOrEmpty(_login.tipoMensaje)) _login.tipoMensaje = "1";
            _login.code = Helpers.RandomString(4);
            Respuesta _respuesta = new Respuesta();
            _respuesta = _processLogic.CodeSend(_login);

            return Json(_respuesta);
        }

        [OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        public JsonResult CodeSendSMS(Login _login)
        {
            _login.user             = _encriptador.DecodeBase64(_login.user);             //decodifica
            _login.usuario_nombre   = _encriptador.DecodeBase64(_login.usuario_nombre);   //decodifica
            _login.usuario_telefono = _encriptador.DecodeBase64(_login.usuario_telefono); //decodifica

            if (string.IsNullOrEmpty(_login.tipoMensaje)) _login.tipoMensaje = "1";
            _login.code = Helpers.RandomString(4);
            Respuesta _respuesta = new Respuesta();
            _respuesta = _processLogic.CodeSendSMS(_login);
            
            return Json(_respuesta);
        }

        public JsonResult CodeEval(Login _login)
        {
            _login.user = _encriptador.DecodeBase64(_login.user);  //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.CodeEval(_login);
            //INICIO-(16-10-2020)-SE GUARDA EN SESSION USUARIO Y CODE
            if (_respuesta.ok == true)
            {
                Session["USER"] = _login.user;
                Session["CODE"] = _login.code;
            }
            //FIN
            
            return Json(_respuesta);
        }

        public JsonResult ChangePasswordAD(Login _login)
        {
            _login.user     = _encriptador.DecodeBase64(_login.user);     //decodifica
            _login.password = _encriptador.DecodeBase64(_login.password); //decodifica

            //JUAN CARLOS RODRIGUZ DONAYRE 25-05-2021 (AGREGADO)
            _login.CodFederada = string.IsNullOrEmpty((string)Session["federada"]) ? "Default" : (string)Session["federada"];
            //
            Respuesta _respuesta = new Respuesta();

            Boolean rs =_conexion.registraEn_usuarioClave(_login);
            if (rs == false)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = "Contraseña repetida";
            }
            else { 
                _respuesta = _processLogic.ChangePasswordAD(_login);
            }
            return Json(_respuesta);
        }


    }
}