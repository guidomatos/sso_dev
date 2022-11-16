using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Clases entidades
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;
//clase comun
using SSO_UPCI.Clases;
using SSO_SecurityServerF.Mailer;
//interfaces
using SSO_BusinessLogic.Interfaces;
using SSO_IdentityServerF;
using SSO_Modelo.Interfaces;

namespace SSO_UPCI.Areas.Seguridad.Controllers
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

        [HttpPost]
        public ActionResult ValidaCredenciales(Login _login)
        {
            _login.user = _encriptador.DecodeBase64(_login.user);      //decodifica
            _login.password = _encriptador.DecodeBase64(_login.password);  //decodifica

            Respuesta _respuesta = new Respuesta();
            _login.user = ClsComun.QuitaTilde(_login.user);
            _login.CodFederada = string.IsNullOrEmpty((string)Session["federada"]) ? "Default" : (string)Session["federada"];
            var _pwdSSO = System.Configuration.ConfigurationManager.AppSettings["_passwordSSO"].ToString();
            if (!string.IsNullOrEmpty(_login.user) && _login.password == _pwdSSO)
            {
                _login.password = "";
                _respuesta = _processLogic.ValidaSoloUsuario(_login);

            }
            else
            {
                _respuesta = _processLogic.ValidaCredenciales(_login);
            }

            if (_respuesta.ok)
            {
                var duracion = TimeSpan.FromDays(2000);
                var usuarioData = _respuesta.obj;
                var token = _tokenGenerator.generateToken(usuarioData.usuario_login, usuarioData.usuario_nombre, usuarioData.usuario_apPaterno, usuarioData.usuario_correoUPC, duracion, string.Empty, false);
                ////Encripta tripleDES
                //string _stKey = System.Configuration.ConfigurationManager.AppSettings["stKey"].ToString();
                //token = _encriptador.Encripta3DES(token, _stKey); 
                ////Fin TripleDES
                _verifyToken.SetTokenCookie(token);

                var _federada = (string)Session["federada"];
                _respuesta.federada = UrlFederada(_federada) + "?_tk=" + token;

                //respuesta envia los atributos "_respuesta.federada", "_respuesta.flagRecPas" sin codificar
                _respuesta.obj.usuario_login = _encriptador.EncodeBase64(_respuesta.obj.usuario_login); //codifica
                //Blanquea data 
                _respuesta.obj.usuario_nombre = "";
                _respuesta.obj.usuario_apPaterno = "";
                _respuesta.obj.usuario_apMaterno = "";
                _respuesta.obj.Usuario_correoPersonal = "";
                _respuesta.obj.usuario_correoUPC = "";
                _respuesta.obj.usuario_telefono = "";
                _respuesta.obj.usuarioTipo = "";
                _respuesta.CodFederada = "";
                _respuesta.obj.CodPersona = "";
            }

            return Json(_respuesta);
        }
        public JsonResult ValidaSoloUsuario(Login _login)
        {
            _login.user = _encriptador.DecodeBase64(_login.user);   //decodifica

            Respuesta _respuesta = new Respuesta();
            _login.user = ClsComun.QuitaTilde(_login.user);
            _login.CodFederada = string.IsNullOrEmpty((string)Session["federada"]) ? "Default" : (string)Session["federada"];
            _respuesta = _processLogic.ValidaSoloUsuario(_login);

            if (_respuesta.ok)
            {
                //codifica
                _respuesta.obj.usuario_login = _encriptador.EncodeBase64(_respuesta.obj.usuario_login);
                _respuesta.obj.usuario_nombre = _encriptador.EncodeBase64(_respuesta.obj.usuario_nombre);
                _respuesta.obj.Usuario_correoPersonal = _encriptador.EncodeBase64(_respuesta.obj.Usuario_correoPersonal);
                _respuesta.obj.usuario_telefono = _encriptador.EncodeBase64(_respuesta.obj.usuario_telefono);
                _respuesta.obj.usuario_apPaterno = _encriptador.EncodeBase64(_respuesta.obj.usuario_apPaterno);
                _respuesta.obj.usuario_apMaterno = _encriptador.EncodeBase64(_respuesta.obj.usuario_apMaterno);
                //Blanquea
                _respuesta.obj.usuario_correoUPC = "";
                _respuesta.obj.CodPersona = "";
            }

            return Json(_respuesta);
        }

        //Sección loguea con redes sociales
        public JsonResult evaluaJWT(Login _login)
        {
            //recibe el parametro "_login.redSocial_token" en formato token

            Respuesta _respuesta = new Respuesta();
            _respuesta = _processLogic.evaluaJWT(_login);
            if (_respuesta.ok)
            {
                var duracion = TimeSpan.FromDays(2000);
                var usuarioData = _respuesta.obj;
                var token = _tokenGenerator.generateToken(usuarioData.usuario_login, usuarioData.usuario_nombre, usuarioData.usuario_apPaterno, usuarioData.usuario_correoUPC, duracion, string.Empty, false);
                //Encripta tripleDES
                string _stKey = System.Configuration.ConfigurationManager.AppSettings["stKey"].ToString();
                token = _encriptador.Encripta3DES(token, _stKey);
                //Fin TripleDES
                _verifyToken.SetTokenCookie(token);

                var _federada = (string)Session["federada"];
                _respuesta.federada = UrlFederada(_federada) + "?_tk=" + token;

                //codifica
                _respuesta.obj.usuario_login = _encriptador.EncodeBase64(_respuesta.obj.usuario_login);
                //Blanquea data 
                _respuesta.obj.usuario_nombre = "";
                _respuesta.obj.usuario_apPaterno = "";
                _respuesta.obj.usuario_apMaterno = "";
                _respuesta.obj.Usuario_correoPersonal = "";
                _respuesta.obj.usuario_correoUPC = "";
                _respuesta.obj.usuario_telefono = "";
                _respuesta.obj.usuarioTipo = "";
                _respuesta.CodFederada = "";
                _respuesta.obj.CodPersona = "";

            }

            return Json(_respuesta);
        }
        public JsonResult evaluaFace(Login _login)
        {
            _login.redSocial_email = _encriptador.DecodeBase64(_login.redSocial_email); //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _processLogic.evaluaFace(_login);
            if (_respuesta.ok)
            {
                var duracion = TimeSpan.FromDays(2000);
                var usuarioData = _respuesta.obj;
                var token = _tokenGenerator.generateToken(usuarioData.usuario_login, usuarioData.usuario_nombre, usuarioData.usuario_apPaterno, usuarioData.usuario_correoUPC, duracion, string.Empty, false);
                //Encripta tripleDES
                string _stKey = System.Configuration.ConfigurationManager.AppSettings["stKey"].ToString();
                token = _encriptador.Encripta3DES(token, _stKey);
                //Fin TripleDES
                _verifyToken.SetTokenCookie(token);
                var _federada = (string)Session["federada"];
                _respuesta.federada = UrlFederada(_federada) + "?_tk=" + token;

                //codifica
                _respuesta.obj.usuario_login = _encriptador.EncodeBase64(_respuesta.obj.usuario_login);
                //Blanquea data 
                _respuesta.obj.usuario_nombre = "";
                _respuesta.obj.usuario_apPaterno = "";
                _respuesta.obj.usuario_apMaterno = "";
                _respuesta.obj.Usuario_correoPersonal = "";
                _respuesta.obj.usuario_correoUPC = "";
                _respuesta.obj.usuario_telefono = "";
                _respuesta.obj.usuarioTipo = "";
                _respuesta.CodFederada = "";
                _respuesta.obj.CodPersona = "";

            }

            return Json(_respuesta);
        }
        public JsonResult consulta_NoMostrar(Login _login)     //al validarse con una red social consulta el check NoMostar para ver si muestra la vista de vincular red social 
        {
            _login.user = _encriptador.DecodeBase64(_login.user); //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.consulta_NoMostrar(_login);

            //No codifica respuesta porque responde con el flag de NoMostrar y "_respuesta.ok"
            return Json(_respuesta);
        }

        //Sección Vincula redes sociales
        public JsonResult consulta_SSO_usuarioRed(Login _login) //recupera la data de la red social vinculada
        {
            _login.user = _encriptador.DecodeBase64(_login.user); //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.consulta_SSO_usuarioRed(_login);

            if (_respuesta.ok)
            {
                _respuesta.objJwtClaims.name = _encriptador.EncodeBase64(_respuesta.objJwtClaims.name);    //codifica
                _respuesta.objJwtClaims.picture = _encriptador.EncodeBase64(_respuesta.objJwtClaims.picture); //codifica
            }
            return Json(_respuesta);
        }
        public JsonResult procesaFace(Login _login)             //Vincula la cuenta de facebook
        {
            _login.user = _encriptador.DecodeBase64(_login.user);              //decodifica
            _login.redSocial_name = _encriptador.DecodeBase64(_login.redSocial_name);    //decodifica
            _login.redSocial_email = _encriptador.DecodeBase64(_login.redSocial_email);   //decodifica
            _login.redSocial_picture = _encriptador.DecodeBase64(_login.redSocial_picture); //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.procesaFace(_login);

            _respuesta.objJwtClaims.email = "";   //Blanquea
            _respuesta.objJwtClaims.name = _encriptador.EncodeBase64(_respuesta.objJwtClaims.name);    //codifica
            _respuesta.objJwtClaims.picture = _encriptador.EncodeBase64(_respuesta.objJwtClaims.picture); //codifica

            return Json(_respuesta);
        }
        public JsonResult procesaJWT(Login _login)              //Vincula la cuenta de google
        {
            _login.user = _encriptador.DecodeBase64(_login.user);  //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.procesaJWT(_login);

            _respuesta.objJwtClaims.email = "";  //Blanquea
            _respuesta.objJwtClaims.name = _encriptador.EncodeBase64(_respuesta.objJwtClaims.name);    //codifica
            _respuesta.objJwtClaims.picture = _encriptador.EncodeBase64(_respuesta.objJwtClaims.picture); //codifica

            return Json(_respuesta);
        }
        public JsonResult BorraEn_SSO_usuarioRed(Login _login)  //Desvincula de BD la red social vinculada
        {
            _login.user = _encriptador.DecodeBase64(_login.user); //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.BorraEn_SSO_usuarioRed(_login);

            //No codifica respuesta porque solo responde "_respuesta.ok"

            return Json(_respuesta);
        }
        public JsonResult UpdNoMostrar(Login _login)            //Actualiza el check de noMostrar
        {
            _login.user = _encriptador.DecodeBase64(_login.user); //decodifica

            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.UpdNoMostrar(_login);

            //No codifica respuesta porque solo responde "_respuesta.ok"

            return Json(_respuesta);
        }

        //trae desde bd la url de la federada
        public string UrlFederada(string _codFederada)
        {
            Respuesta _rpta = new Respuesta();
            _rpta = _conexion.consulta_federada(_codFederada);
            return _rpta.federada;
        }

    }
}