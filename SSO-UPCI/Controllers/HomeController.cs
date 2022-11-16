using System;
using System.Web;
using System.Web.Mvc;
using SSO_BusinessLogic;
using SSO_BusinessLogic.Interfaces;
using SSO_IdentityServerF;
using SSO_Modelo.Clases;
using SSO_Modelo.Interfaces;
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;
using SSO_SecurityServerF.Mailer;

using Newtonsoft.Json; //PRY-SSO (05/12/2020)
using System.Net.Http; //PRY-SSO (05/12/2020)
using System.Collections.Generic; //PRY-SSO (05/12/2020)
using System.Net.Http.Formatting;//PRY-SSO (05/12/2020)

//clase comun
using SSO_UPCI.Clases;

namespace SSO_UPCI.Controllers
{
    public class HomeController : Controller
    {
        public readonly IConexion _conexion;
        private readonly IVerifyToken _verifyToken;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IProcessLogic _processLogic;

        public HomeController(IVerifyToken verifyToken, IConexion conexion, ITokenGenerator tokenGenerator, IProcessLogic processLogic)
        {
            _verifyToken = verifyToken;
            _conexion = conexion;
            _tokenGenerator = tokenGenerator;
            _processLogic = processLogic;
        }
        
        public ActionResult Verifica(string _url)
        {
            var _redir = "";
            if (!string.IsNullOrEmpty(_url))
            {
                HttpCookie cookie = Request.Cookies.Get("tokenSSO");
                if (cookie == null)
                {
                    _redir = _url + "?_tk=KO";
                }
                else
                {
                    _redir = _url + "?_tk=OK&_hr=" + DateTime.Now.ToString("hh:mm");
                }
            }
            return Redirect(_redir);
        }
        
        public ActionResult Verificaget(string _url) //recibe el parametro _url con el simbolo "?". Le respondemos con el simbolo "&"
        {
            var _redir = "";
            if (!string.IsNullOrEmpty(_url))
            {
                HttpCookie cookie = Request.Cookies.Get("tokenSSO");
                if (cookie == null)
                {
                    _redir = _url + "&_tk=KO";
                }
                else
                {
                    _redir = _url + "&_tk=OK&_hr=" + DateTime.Now.ToString("hh:mm");
                }
            }
            return Redirect(_redir);
        }
        
        public ActionResult Index(string _f)
        {
            string pathCodeSend = Url.Action("Proceso/Home/CodeSend");
            Response.RemoveOutputCacheItem(pathCodeSend);

            string pathCodeSendSMS = Url.Action("Proceso/Home/CodeSendSMS");
            Response.RemoveOutputCacheItem(pathCodeSendSMS);

            //Administra federadas
            Session["federada"] = _f;
            if (!string.IsNullOrEmpty(_f))
            {
                string _token = "";
                string _federada = "";
                HttpCookie cookie = Request.Cookies.Get("tokenSSO");
                if (cookie != null)
                {
                    _token = cookie.Value;
                    _federada = UrlFederada(_f);
                    _federada = _federada + "?_tk=" + _token;

                    return Redirect(_federada);
                }
            }
            //Administra federadas-End
            return View();
        }
        
        public ActionResult Login()
        {
            return View("login");
        }
        
        public ActionResult VinculateAccounts()
        {
            //Juan Carlos Rodriguez 26-11-2021 (agregado)
            HttpCookie cookie = Request.Cookies.Get("tokenSSO");
            if (cookie == null)
            {
                return RedirectToAction("Login");
            }
            //Fin

            return View("vinculate_accounts");
        }
        
        public ActionResult RecoverPass(string _f)
        {
            //JUAN CARLOS RODRIGUEZ DONAYRE 25-05-2021 (AGREGADO) 
            //PARA MANEJAR EL LLAMADO DE ESTE MÉTODO DESDE UNA FEDERADA
            if (!string.IsNullOrEmpty(_f)) Session["federada"] = _f;
            //FIN
            ViewBag.urlActData = System.Configuration.ConfigurationManager.AppSettings["urlActData"].ToString();
            return View("recover_pass");
        }
        
        public ActionResult RecoverMethod()
        {
            return View("recover_method");
        }
        
        public ActionResult RecoverConfirmSms()
        {
            return View("recover_confirm_sms");
        }
        
        public ActionResult RecoverConfirmEmail()
        {
            return View("recover_confirm_email");
        }
        
        public ActionResult RecoverVerification()
        {
            return View("recover_verification");
        }
        
        public ActionResult CreatePassword()
        {
            //INICIO-(16-10-2020)-SE EVALUA EL USUARIO Y CODE
            string _usr = (string)Session["USER"];
            string _code = (string)Session["CODE"];
            if (string.IsNullOrEmpty(_usr) || string.IsNullOrEmpty(_code))
            {
                return RedirectToAction("EndSession");
            }
            Login _login = new Login() { user= _usr, code= _code };
            Respuesta _respuesta = new Respuesta();
            _respuesta = _conexion.CodeEval(_login);
            if (_respuesta.ok == false)
            {
                RedirectToAction("EndSession");
            }
            //FIN
            return View("create_pass");
        }
        
        public ActionResult UpdatedPassword()
        {
            return View("updated_pass");
        }
        
        public ActionResult EndSession(string _f)
        {
            _verifyToken.KillToken();
            //Response.Cookies["tokenSSO"].Expires = DateTime.Now.AddDays(-1);
            //ViewBag.Message = "Sesión finalizada";
            ViewBag.federada = _f;
            return View("end_session");
        }
        
        public ActionResult AppFederada()
        {
            if (_verifyToken.TokenCheker())
            {
                return View("app_federada");
              
            }

            return RedirectToAction("index");
        }
        
        public string UrlFederada(string _codFederada)
        {
            Respuesta _rpta = new Respuesta();
            _rpta = _conexion.consulta_federada(_codFederada);
            return _rpta.federada;
        }

        //public class objClaims
        //{
        //    public DTOHeader DTOHeader { get; set; }
        //    public List<ListaDTOUsuarios> ListaDTOUsuarios { get; set; }
        //}
        //public class DTOHeader
        //{
        //    public string CodigoRetorno { get; set; }
        //    public string DescRetorno { get; set; }
        //}
        //public class ListaDTOUsuarios
        //{ 
        //    public DTOUsuarioCabecera DTOUsuarioCabecera { get; set; }
        //    public List<DetalleDTOUsuario> DetalleDTOUsuario { get; set; }

        //}
        //public class DTOUsuarioCabecera
        //{
        //    public string CodLineaNegocio { get; set; }
        //    public string CodUsuario { get; set; }
        //}
        //public class DetalleDTOUsuario
        //{
        //    public string CodPersona { get; set; }
        //    public string ApellidoPaterno { get; set; }
        //    public string ApellidoMaterno { get; set; }
        //    public string Nombres { get; set; }
        //    public string ApellidoPaternoPropio { get; set; }
        //    public string ApellidoMaternoPropio { get; set; }
        //    public string NombresPropio { get; set; }
        //    public string TipoDocumento { get; set; }
        //    public string DocumentoIdentidad { get; set; }
        //    public string EstadoPersona { get; set; }
        //    public string CodTipoUsuario { get; set; }
        //    public string DesTipoUsuario { get; set; }
        //    public string Administrativo { get; set; }
        //    public string TipoPersona { get; set; }
        //    public string EstadoUsuario { get; set; }
        //    public string Email { get; set; }
        //    public string EmailAlterno { get; set; }
        //    public string CodArea { get; set; }
        //    public string CodAreaAcad { get; set; }
        //    public string RequiereAuditoria { get; set; }
        //    public string ValidaBd { get; set; }
        //    public string ClaveSecreta { get; set; }
        //    public string FechaClave { get; set; }
        //    public string TiempoExpira { get; set; }
        //    public string MaxSesiones { get; set; }
        //    public string IntentosFallidos { get; set; }
        //    public string MotivoDesactivacion { get; set; }
        //    public string RequierePermiso { get; set; }
        //    public string UsuarioLibre { get; set; }
        //    public string ConsultaLibre { get; set; }
        //    public string IndEncExaOnline { get; set; }
        //    public string PrimerIngreso { get; set; }
        //    public string FechaBloqueo { get; set; }
        //    public string FechaIntentos { get; set; }
        //    public string OrigenUpdate { get; set; }
        //    public string FechaCreacion { get; set; }
        //    public string UsuarioCreador { get; set; }
        //    public string FechaModificacion { get; set; }
        //    public string UsuarioModificador { get; set; }
        //    public string EsDirectorCarrera { get; set; }
        //    public string TelefonoMovil { get; set; }

        //}
        //public class DataCRM
        //{
        //    public string Route { get; set; }
        //    public string CodigoPersona { get; set; }
        //    public string CelularSSO { get; set; }
        //    public string EmailSSO { get; set; }
        //}
        //public class DataCRMrpta
        //{
        //    public string Operacion { get; set; }
        //    public string Id { get; set; }
        //}
        //public class DataCRMget
        //{
        //    public string CelularSSO { get; set; }
        //    public string EmailSSO { get; set; }
        //}

        //[HttpPost]
        //public ActionResult ValidaCredenciales(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _login.user        = ClsComun.QuitaTilde(_login.user);
        //    _login.CodFederada = string.IsNullOrEmpty((string)Session["federada"]) ? "Default" : (string)Session["federada"];
        //    var _pwdSSO = System.Configuration.ConfigurationManager.AppSettings["_passwordSSO"].ToString();
        //    if (!string.IsNullOrEmpty(_login.user) && _login.password == _pwdSSO)
        //    {
        //        _login.password = "";
        //        _respuesta = _processLogic.ValidaSoloUsuario(_login);

        //        var base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.usuario_login);
        //        _respuesta.obj.usuario_login = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        //        base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.usuario_nombre);
        //        _respuesta.obj.usuario_nombre = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        //        base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.usuario_apPaterno);
        //        _respuesta.obj.usuario_apPaterno = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        //        base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.Usuario_correoPersonal);
        //        _respuesta.obj.Usuario_correoPersonal = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        //        base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.usuario_correoUPC);
        //        _respuesta.obj.usuario_correoUPC = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        //        base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.usuario_telefono);
        //        _respuesta.obj.usuario_telefono = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        //        base64EncodedBytes = System.Convert.FromBase64String(_respuesta.obj.CodPersona);
        //        _respuesta.obj.CodPersona = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        //    }
        //    else
        //    {
        //        _respuesta = _processLogic.ValidaCredenciales(_login);
        //    }

        //    if (_respuesta.ok)
        //    {
        //        var duracion = TimeSpan.FromDays(2000);
        //        var usuarioData = _respuesta.obj;
        //        var token = _tokenGenerator.generateToken(usuarioData.usuario_login,usuarioData.usuario_nombre,usuarioData.usuario_apPaterno,usuarioData.usuario_correoUPC, duracion,string.Empty,false);
        //        _verifyToken.SetTokenCookie(token);

        //        var _federada = (string)Session["federada"];
        //        _respuesta.federada = UrlFederada(_federada) + "?_tk=" + token;
        //    }
        //    return Json(_respuesta);
        //}
        //public JsonResult ValidaSoloUsuario(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _login.user        = ClsComun.QuitaTilde(_login.user);
        //    _login.CodFederada = string.IsNullOrEmpty((string)Session["federada"]) ? "Default" : (string)Session["federada"];
        //    _respuesta = _processLogic.ValidaSoloUsuario(_login);
        //    return Json(_respuesta);
        //}
        //[OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        //public JsonResult CodeSend(Login _login)
        //{
        //    if (string.IsNullOrEmpty(_login.tipoMensaje)) _login.tipoMensaje = "1";
        //    _login.code = Helpers.RandomString(4);
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _processLogic.CodeSend(_login);
        //    return Json(_respuesta);
        //}
        //[OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        //public JsonResult CodeSendSMS(Login _login)
        //{
        //    if (string.IsNullOrEmpty(_login.tipoMensaje)) _login.tipoMensaje = "1";
        //    _login.code = Helpers.RandomString(4);
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _processLogic.CodeSendSMS(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult CodeEval(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.CodeEval(_login);
        //    //INICIO-(16-10-2020)-SE GUARDA EN SESSION USUARIO Y CODE
        //    if ( _respuesta.ok == true)
        //    {
        //        Session["USER"] = _login.user;
        //        Session["CODE"] = _login.code;
        //    }
        //    //FIN
        //    return Json(_respuesta);
        //}
        //public JsonResult ChangePasswordAD(Login _login)
        //{
        //    //JUAN CARLOS RODRIGUZ DONAYRE 25-05-2021 (AGREGADO)
        //    _login.CodFederada = string.IsNullOrEmpty((string)Session["federada"]) ? "Default" : (string)Session["federada"];
        //    //
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _processLogic.ChangePasswordAD(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult procesaJWT(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.procesaJWT(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult procesaFace(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.procesaFace(_login);
        //    return Json(_respuesta);
        //}

        //public JsonResult consulta_SSO_usuarioRed(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.consulta_SSO_usuarioRed(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult UpdNoMostrar(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.UpdNoMostrar(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult consulta_NoMostrar(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.consulta_NoMostrar(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult BorraEn_SSO_usuarioRed(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _conexion.BorraEn_SSO_usuarioRed(_login);
        //    return Json(_respuesta);
        //}
        //public JsonResult evaluaJWT(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _processLogic.evaluaJWT(_login);
        //    if (_respuesta.ok)
        //    {
        //        var duracion = TimeSpan.FromDays(2000);
        //        var usuarioData = _respuesta.obj;
        //        var token = _tokenGenerator.generateToken(usuarioData.usuario_login, usuarioData.usuario_nombre, usuarioData.usuario_apPaterno, usuarioData.usuario_correoUPC, duracion, string.Empty, false);
        //        _verifyToken.SetTokenCookie(token);
        //        var _federada = (string)Session["federada"];
        //        _respuesta.federada = UrlFederada(_federada) + "?_tk=" + token;
        //    }
        //    return Json(_respuesta);
        //}
        //public JsonResult evaluaFace(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    _respuesta = _processLogic.evaluaFace(_login);
        //    if (_respuesta.ok)
        //    {
        //        var duracion = TimeSpan.FromDays(2000);
        //        var usuarioData = _respuesta.obj;
        //        var token = _tokenGenerator.generateToken(usuarioData.usuario_login, usuarioData.usuario_nombre, usuarioData.usuario_apPaterno, usuarioData.usuario_correoUPC, duracion, string.Empty, false);
        //        _verifyToken.SetTokenCookie(token);
        //        var _federada = (string)Session["federada"];
        //        _respuesta.federada = UrlFederada(_federada) + "?_tk=" + token;
        //    }
        //    return Json(_respuesta);
        //}
        //public ActionResult UpdateData(string _f)
        //{
        //    //JUAN CARLOS RODRIGUEZ DONAYRE 03-06-2021 (AGREGADO) 
        //    //PARA MANEJAR EL LLAMADO DE ESTE MÉTODO DESDE UNA FEDERADA
        //    if (!string.IsNullOrEmpty(_f)) Session["federada"] = _f;
        //    //FIN

        //    //Juan Carlos Rodriguez 26-11-2021 (agregado)
        //    HttpCookie cookie = Request.Cookies.Get("tokenSSO");
        //    if (cookie == null)
        //    {
        //        return RedirectToAction("Login");
        //    }
        //    //Fin

        //    return View();
        //}
        //public ActionResult updateDataConfirm()
        //{
        //    //Juan Carlos Rodriguez 26-11-2021 (agregado)
        //    HttpCookie cookie = Request.Cookies.Get("tokenSSO");
        //    if (cookie == null)
        //    {
        //        return RedirectToAction("Login");
        //    }
        //    //Fin

        //    return View();
        //}
        //public ActionResult updateDataEnd()
        //{
        //    //JUAN CARLOS RODRIGUEZ DONAYRE 03-06-2021 (AGREGADO) 
        //    //Administra federadas
        //    var _f = (string)Session["federada"];
        //    string _federada = UrlFederada(_f);

        //    HttpCookie cookie = Request.Cookies.Get("tokenSSO");
        //    if (cookie != null)
        //    {
        //        string _token = cookie.Value;
        //        _federada = _federada + "?_tk=" + _token;
        //    }
        //    else 
        //    { 
        //        return RedirectToAction("Login");
        //    }

        //    ViewBag.federada = _federada;
        //    //FIN

        //    return View();
        //}
        //public JsonResult   updateDataCRM(Login _login)
        //{
        //    Respuesta _respuesta = new Respuesta() { ok=false, mensaje="" };
        //    try
        //    {
        //        string CodPersona = "";

        //        //Trae credenciales del web.config 
        //        var _ApiDominio    = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
        //        var _ApiCredencial = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

        //        //Juan Carlos Rodríguez Donayre (20/10/2021)
        //        // Si el switch es 1 toma la contraseña desde la tabla de variables
        //        string switchReadPwdFromTable = System.Configuration.ConfigurationManager.AppSettings["switchReadPwdFromTable"];
        //        if (switchReadPwdFromTable == "1")
        //        {
        //            Respuesta res = _conexion.consulta_Variables_Id(5);
        //            _ApiCredencial = res.mensaje;
        //        }
        //        //Fin

        //        //API - Busca el código de persona
        //        HttpClient clienteHTTP = new HttpClient();
        //        clienteHTTP.BaseAddress = new Uri(_ApiDominio);
        //        clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + _ApiCredencial); // token generado en PostMan

        //        var request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=U&CodUsuario=" + _login.user.ToUpper()).Result;
        //        if (request.IsSuccessStatusCode)
        //        {
        //            var resultString = request.Content.ReadAsStringAsync().Result;
        //            var _objClaims = JsonConvert.DeserializeObject<objClaims>(resultString);
        //            if (_objClaims.ListaDTOUsuarios.Count>0) 
        //            {
        //                CodPersona = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
        //            }
        //            else
        //            {
        //                request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=E&CodUsuario=" + _login.user.ToUpper()).Result;
        //                if (request.IsSuccessStatusCode) 
        //                {
        //                    var _resultString = request.Content.ReadAsStringAsync().Result;
        //                    _objClaims = JsonConvert.DeserializeObject<objClaims>(_resultString);
        //                    CodPersona = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
        //                }
        //                else
        //                {
        //                    throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
        //        }
        //        //--FIN

        //        //API - Actualiza Datos en CRM
        //        DataCRM _dataCRM = new DataCRM();
        //        _dataCRM.Route         = "1";
        //        _dataCRM.CodigoPersona = CodPersona;
        //        _dataCRM.CelularSSO    = _login.usuario_telefono;
        //        _dataCRM.EmailSSO      = _login.Usuario_correoPersonal;

        //        request = clienteHTTP.PostAsync("/v3/crm/Contacto", _dataCRM, new JsonMediaTypeFormatter()).Result;
        //        if (request.IsSuccessStatusCode)
        //        {
        //            var resultString = request.Content.ReadAsStringAsync().Result;
        //            var _DataCRM = JsonConvert.DeserializeObject<DataCRMrpta>(resultString);
        //            var _op = _DataCRM.Operacion;
        //            if (_op == "2") _respuesta.ok = true;
        //        }
        //        else
        //        {
        //            throw new ApplicationException("API /v3/crm/Contacto no actualizó la información.");
        //        }
        //        //--FIN

        //        //Registra la fecha de actualización de datos en la tabla SSO_usuario. Cambio ralizado el 25/10/2021
        //        if (_respuesta.ok == true)
        //        {
        //            Boolean _result = _conexion.registraUltActDatos(_login);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _respuesta.ok      = false;
        //        _respuesta.mensaje = ex.Message;
        //    }

        //    return Json(_respuesta);
        //}
        //public JsonResult   getDataCRM(Login _login) 
        //{
        //    Respuesta _respuesta = new Respuesta() { ok = false, mensaje = "" };
        //    try
        //    {
        //        string CodPersona = "";

        //        //Trae credenciales del web.config 
        //        var _ApiDominio = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
        //        var _ApiCredencial = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

        //        //Juan Carlos Rodríguez Donayre (20/10/2021)
        //        // Si el switch es 1 toma la contraseña desde la tabla de variables
        //        string switchReadPwdFromTable = System.Configuration.ConfigurationManager.AppSettings["switchReadPwdFromTable"];
        //        if (switchReadPwdFromTable == "1")
        //        {
        //            Respuesta res = _conexion.consulta_Variables_Id(5);
        //            _ApiCredencial = res.mensaje;
        //        }
        //        //Fin

        //        //API - Busca el código de persona
        //        HttpClient clienteHTTP = new HttpClient();
        //        clienteHTTP.BaseAddress = new Uri(_ApiDominio);
        //        clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + _ApiCredencial); // token generado en PostMan

        //        var request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=U&CodUsuario=" + _login.user.ToUpper()).Result;
        //        if (request.IsSuccessStatusCode)
        //        {
        //            var resultString = request.Content.ReadAsStringAsync().Result;
        //            var _objClaims = JsonConvert.DeserializeObject<objClaims>(resultString);
        //            if (_objClaims.ListaDTOUsuarios.Count > 0)
        //            {
        //                CodPersona = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
        //            }
        //            else
        //            {
        //                request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=E&CodUsuario=" + _login.user.ToUpper()).Result;
        //                if (request.IsSuccessStatusCode)
        //                {
        //                    var _resultString = request.Content.ReadAsStringAsync().Result;
        //                    _objClaims = JsonConvert.DeserializeObject<objClaims>(_resultString);
        //                    CodPersona = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
        //                }
        //                else
        //                {
        //                    throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
        //        }
        //        //--FIN

        //        //API - Trae de CRM los campos CelularSSO y EmailSSO
        //        request = clienteHTTP.GetAsync("/v3/crm/Contacto?TipoBusqueda=5&ValorBusqueda=" + CodPersona + "&Route=1").Result;
        //        if (request.IsSuccessStatusCode)
        //        {
        //            var resultString = request.Content.ReadAsStringAsync().Result;
        //            var _DataCRM = JsonConvert.DeserializeObject<DataCRMget>(resultString);
        //            _respuesta.celular = _DataCRM.CelularSSO;
        //            _respuesta.correo  = _DataCRM.EmailSSO;
        //            _respuesta.ok = true;
        //        }
        //        else
        //        {
        //            throw new ApplicationException("API /v3/crm/Contacto no devolvió información.");
        //        }
        //        //--FIN

        //    }
        //    catch (Exception ex)
        //    {
        //        _respuesta.ok = false;
        //        _respuesta.mensaje = ex.Message;
        //    }

        //    return Json(_respuesta);

        //}
        //public ActionResult CargaUsuarioTipo()
        //{
        //    return View();
        //}
        //public JsonResult ProcesoUsuarioTipo()
        //{
        //    Respuesta _respuesta = new Respuesta();
        //    Conexion  _cnx = new Conexion();
        //    try
        //    {
        //        var _list = _cnx.ListaDeUsuarios();
        //        foreach (var _reg in _list)
        //        {
        //            Login _login = new Login();
        //            _login.user = _reg.usuario_login;
        //            _login.usuarioTipo = _processLogic.Api_Usuarios_Tipo(_reg.usuario_login);
        //            _cnx.registraUsuarioTipo(_login);
        //        }
        //        _respuesta.ok = true;
        //    }
        //    catch(Exception ex)
        //    {
        //        _respuesta.ok = false;
        //        _respuesta.mensaje = ex.Message;
        //    }
        //    return Json(_respuesta);

        //}
    }
}
