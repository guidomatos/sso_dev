using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using SSO_BusinessLogic.Interfaces;
using SSO_Modelo.Interfaces;
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;
using SSO_SecurityServerF.Mailer;

using Newtonsoft.Json; //JUAN_CARLOS_RODRIGUEZ 18-03-2021
using System.Net.Http; //JUAN_CARLOS_RODRIGUEZ 18-03-2021
using System.Net.Http.Formatting;//JUAN_CARLOS_RODRIGUEZ 18-03-2021 

namespace SSO_BusinessLogic
{
    //JUAN CARLOS RODRIGUEZ 22/07/2021 (agregado)
    public class U_objClaims
    {
        public U_DTOHeader              DTOHeader { get; set; }
        public List<U_ListaDTOUsuarios> ListaDTOUsuarios { get; set; }
    }
    public class U_DTOHeader
    {
        public string CodigoRetorno { get; set; }
        public string DescRetorno { get; set; }
    }
    public class U_ListaDTOUsuarios
    {
        public U_DTOUsuarioCabecera      DTOUsuarioCabecera { get; set; }
        public List<U_DetalleDTOUsuario> DetalleDTOUsuario { get; set; }

    }
    public class U_DTOUsuarioCabecera
    {
        public string CodLineaNegocio { get; set; }
        public string CodUsuario { get; set; }
    }
    public class U_DetalleDTOUsuario
    {
        public string CodPersona { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaternoPropio { get; set; }
        public string ApellidoMaternoPropio { get; set; }
        public string NombresPropio { get; set; }
        public string TipoDocumento { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string EstadoPersona { get; set; }
        public string CodTipoUsuario { get; set; }
        public string DesTipoUsuario { get; set; }
        public string Administrativo { get; set; }
        public string TipoPersona { get; set; }
        public string EstadoUsuario { get; set; }
        public string Email { get; set; }
        public string EmailAlterno { get; set; }
        public string CodArea { get; set; }
        public string CodAreaAcad { get; set; }
        public string RequiereAuditoria { get; set; }
        public string ValidaBd { get; set; }
        public string ClaveSecreta { get; set; }
        public string FechaClave { get; set; }
        public string TiempoExpira { get; set; }
        public string MaxSesiones { get; set; }
        public string IntentosFallidos { get; set; }
        public string MotivoDesactivacion { get; set; }
        public string RequierePermiso { get; set; }
        public string UsuarioLibre { get; set; }
        public string ConsultaLibre { get; set; }
        public string IndEncExaOnline { get; set; }
        public string PrimerIngreso { get; set; }
        public string FechaBloqueo { get; set; }
        public string FechaIntentos { get; set; }
        public string OrigenUpdate { get; set; }
        public string FechaCreacion { get; set; }
        public string UsuarioCreador { get; set; }
        public string FechaModificacion { get; set; }
        public string UsuarioModificador { get; set; }
        public string EsDirectorCarrera { get; set; }
        public string TelefonoMovil { get; set; }

    }

    //FIN

    public class sms
    {
        public int estado { get; set; }
        public string codigo { get; set; }
    }

    //JUAN_CARLOS_RODRIGUEZ 18/03/2021 (agregado)
    public class Auth
    {
        public string alias { get; set; }
        public string password { get; set; }
    }
    public class objClaims
    {
        public string AdUser { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public string ok { get; set; }
    }
    //FIN
    //JUAN_CARLOS_RODRIGUEZ 22/03/2021 (agregado)
    public class AuthAlias
    {
        public string alias { get; set; }
    }
    public class objClaimsDatosUsuario
    {
        public DTOHeader DTOHeader { get; set; }
        public DTODatosUsuario DTOUsuario { get; set; }
    }
    public class DTOHeader
    {
        public string CodigoRetorno { get; set; }
        public string MsjRetorno { get; set; }
        public string EstadoRetorno { get; set; }
    }
    public class DTODatosUsuario
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string EmailUPC { get; set; }
        public string Anexo { get; set; }
        public string CN { get; set; }
        public string OU { get; set; }
        public string NombreUsuario { get; set; }

    }
    //FIN
    //JUAN_CARLOS_RODRIGUEZ 23/03/2021 (agregado)
    public class AuthChangePass
    {
        public string usuario { get; set; }
        public string password { get; set; }
    }
    //FIN

    //JUAN_CARLOS_RODRIGUEZ 29/04/2021 (agregado)
    public class ERes
    {
        public bool ok;
        public string msg;
        public int code;
        public EAdUser AdUser { get; set; }
    }
    public class EAdUser
    {
        public string Email { get; set; }
        public string EmailUPC { get; set; }
        //public string EmailPersonal { get; set; }
        public string Anexo { get; set; }
        //public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        //public string CN { get; set; }
        //public string OU { get; set; }
        //public string SamAcountName { get; set; }

    }
    //FIN
    public class ProcessLogic : IProcessLogic
    {
        private readonly IConexion _conexion;
        public ProcessLogic(IConexion conexion)
        {
            _conexion = conexion;
        }
        public string MensajesAD(int _code)
        {
            string Observa = "";
            switch (_code)
            {
                case 0:
                    Observa = "Conexión establecida exitosamente.";
                    break;
                case 1:
                    Observa = "La cuenta de usuario ha sido bloqueda por exceder el número de intentos fallidos. La cuenta será desbloqueada automáticamente dentro de 5 minutos..";
                    break;
                case 2:
                    Observa = "El usuario ingresado se encuentra Inactivo, por favor notifique a IT Service.";
                    break;
                case 3:
                    Observa = "El usuario o contraseña es incorrecto, inténtelo nuevamente. Recuerde que luego de 5 intentos fallidos su cuenta se bloqueará.";
                    break;
                case 4:
                    Observa = "Es necesario que cambie su contraseña desde la opción 'Olvidaste tu Contraseña'.";
                    break;
                case 5:
                    Observa = "Su contraseña ha caducado. Es necesario que cambie su contraseña desde la opción 'Olvidaste tu Contraseña'.";
                    break;
                case 6:
                    Observa = "El usuario ingresado no existe. De persistir el problema, por favor notifique a IT Service.";
                    break;
                default:
                    Observa = "Ocurrió algo inesperado!!!, inténtelo nuevamente más tarde.";
                    break;
            }
            return Observa;
        }
        public UsuarioAD consultaAD(Login _login)
        {
            UsuarioAD _usuarioAD = new UsuarioAD();
            var _AD = new AD();
            Boolean _result = false;

            if (!string.IsNullOrEmpty(_login.password))
            {
                var r = _AD.auth(_login.user, _login.password);
                _result = r.Result.ok;
                _usuarioAD.usuario_code = r.Result.code;
                _usuarioAD.usuario_msg = MensajesAD(r.Result.code);
            }
            else
            {
                _result = true;
            }

            if (_result)
            {
                var _r = _AD.getUser(_login.user);
                _result = _r.Result.ok;
                _usuarioAD.usuario_code = _r.Result.code;
                _usuarioAD.usuario_msg = MensajesAD(_r.Result.code);

                if (_result)
                {
                    _usuarioAD.usuario_login = _login.user;
                    _usuarioAD.usuario_nombre = _r.Result.AdUser.Name;
                    _usuarioAD.usuario_apPaterno = _r.Result.AdUser.LastName;
                    _usuarioAD.usuario_apMaterno = " ";
                    _usuarioAD.Usuario_correoPersonal = _r.Result.AdUser.Email;
                    _usuarioAD.usuario_correoUPC = _r.Result.AdUser.EmailUPC;
                    _usuarioAD.usuario_telefono = _r.Result.AdUser.Phone;
                    _usuarioAD.usuario_code = _r.Result.code;
                    _usuarioAD.usuario_msg = MensajesAD(_r.Result.code);
                }
            }
            return _usuarioAD;
        }
        public Respuesta ChangePasswordAD(Login _login)
        {
            Respuesta _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user) && !string.IsNullOrEmpty(_login.password))
                {
                    //JUAN CARLOS RODRIGUEZ DONAYRE 03-05-2021 (comentado)
                    //var _AD = new AD();
                    //var _res = _AD.ChangePasswordAD(_login.user, _login.password);
                    //_respuesta.ok = _res.Item1;
                    //_respuesta.mensaje = _res.Item2;
                    //FIN

                    ////JUAN CARLOS RODRIGUEZ DONAYRE 03-05-2021 USO DE APIS Y LECTURA DE FEDERA DESDE BD (agregado)
                    //Respuesta _rpta = new Respuesta();
                    //_rpta = _conexion.consulta_federada(_login.CodFederada);
                    //if (_rpta.federada_api == "A")
                    //{
                    //    _respuesta = ChangePasswordAD_API_A(_login);
                    //}
                    //else if (_rpta.federada_api == "O")
                    //{
                    //    _respuesta = ChangePasswordAD_API(_login);
                    //}
                    ////FIN

                    ////JUAN CARLOS RODRIGUEZ DONAYRE 03-09-2021 (AGREGADO)
                    ////SI EL INTENTO DE CAMBIO DE PWD EN AZURE FALLÓ Y EL CODIGO DE FEDERADA SE ENCUENTRA EN CADENA
                    ////ENTONCES SE LOGUEA CON EL AD ONPREMISES
                    //Boolean found = false;
                    //string Cadena = ConfigurationManager.AppSettings["switchAD"];
                    //string[] A = Cadena.Split(',');
                    //for (int i = 0; i <= A.Length - 1; i++)
                    //{
                    //    if (A[i] == _login.CodFederada)
                    //    {
                    //        found = true;
                    //    }
                    //}
                    //if ((_rpta.federada_api == "A" && _respuesta.code != 0) && found)
                    //{
                    //    _respuesta = ChangePasswordAD_API(_login);
                    //}
                    ////FIN

                    _respuesta = ChangePasswordAD_API(_login);

                }
                //INICIO-(28-10-2020) REGISTRA FLAG A FALSO PARA NO RECUPERAR CONTRASEÑA AL INICIO SIGUIENTE
                if (_respuesta.ok == true)
                {
                    var _result = _conexion.registraFlag(_login);
                }
                //FIN
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta ValidaCredenciales(Login _login)
        {
            Respuesta Respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user) && !string.IsNullOrEmpty(_login.password))
                {
                    //JUAN_CARLOS_RODRIGUEZ 29/04/2021 (comentado)
                    //UsuarioAD _UsuarioAD = consultaAD(_login);
                    //FIN

                    //JUAN_CARLOS_RODRIGUEZ 29/04/2021 USO DE APIS Y LECTURA DE FEDERA DESDE BD (agregado)
                    UsuarioAD UsuarioAD = new UsuarioAD();
                    Respuesta rpta = new Respuesta();
                    rpta = _conexion.consulta_federada(_login.CodFederada);
                    if (rpta.federada_api == "A")
                    {
                        UsuarioAD = Auth_Azure(_login);
                    }
                    else if (rpta.federada_api == "O")
                    {
                        UsuarioAD = Auth_OnPremises(_login);
                    }
                    else
                    {
                        UsuarioAD = ConsultaAD_Debug(_login);
                    }
                    //FIN

                    //JUAN CARLOS RODRIGUEZ DONAYRE 03-09-2021 (AGREGADO)
                    //SI EL INTENTO DE CONEXION AL AD DE AZURE FALLÓ Y EL CODIGO DE FEDERADA SE ENCUENTRA EN CADENA
                    //ENTONCES SE LOGUEA CON EL AD ONPREMISES
                    Boolean found = false;
                    string Cadena = ConfigurationManager.AppSettings["switchAD"];
                    string[] A = Cadena.Split(',');
                    for (int i = 0; i <= A.Length - 1; i++)
                    {
                        if (A[i] == _login.CodFederada)
                        {
                            found = true;
                        }
                    }
                    if ((rpta.federada_api == "A" && UsuarioAD.usuario_code != 0) && found)
                    {
                        UsuarioAD = Auth_OnPremises(_login);
                    }
                    //FIN

                    if (UsuarioAD.usuario_code != 0)
                    {
                        Respuesta.ok = false;
                        Respuesta.mensaje = UsuarioAD.usuario_msg;
                        Respuesta.obj = UsuarioAD;
                    }
                    else
                    {
                        Respuesta.ok = true;
                        Respuesta.mensaje = "";
                        Respuesta.obj = UsuarioAD;   //devolvemos la clase usuarioAD con datos
                    }
                }

                if (Respuesta.ok == true)  //credenciales correctas, actualiza tablas involucradas en el login
                {
                    //INICIO (12-11-2020) ENVIO CODIGO DE FEDERADA
                    Respuesta.CodFederada = _login.CodFederada;
                    //FIN
                    Respuesta = _conexion.RegistrarValidaCredenciales(Respuesta);
                }

                //INICIO-(28-10-2020) CONSULTA FLAG RECUPERAR CONTRASEÑA
                if (Respuesta.ok == true)
                {
                    Respuesta.flagRecPas = _conexion.consultaFlag(_login);
                }
                //FIN
            }
            catch (Exception ex)
            {
                Respuesta.ok = false;
                Respuesta.mensaje = ex.Message;
            }

            return Respuesta;
        }
        public Respuesta ValidaSoloUsuario(Login _login)
        {
            Respuesta Respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {
                    //JUAN_CARLOS_RODRIGUEZ 03/05/2021 (comentado)
                    //UsuarioAD UsuarioAD  = consultaAD(_login);
                    //FIN

                    //JUAN CARLOS RODRIGUEZ DONAYRE 13/09/2021 (AGREGADO)
                    UsuarioAD UsuarioAD = Api_Usuarios(_login);
                    //FIN

                    if (UsuarioAD.usuario_code != 0)
                    {
                        Respuesta.ok = false;
                        Respuesta.mensaje = UsuarioAD.usuario_msg;
                        Respuesta.obj = UsuarioAD;   //pasamos clase usuarioAD
                    }
                    else
                    {
                        Respuesta.ok = true;
                        Respuesta.mensaje = "";
                        Respuesta.obj = UsuarioAD;   //pasamos clase usuarioAD
                    }
                }
                if (Respuesta.ok == true)  //credenciales correctas, actualiza tablas involucradas en el login
                {
                    //INICIO (12-11-2020) ENVIO CODIGO DE FEDERADA
                    Respuesta.CodFederada = _login.CodFederada;
                    //FIN
                    Respuesta = _conexion.RegistrarValidaCredenciales(Respuesta);
                }
            }
            catch (Exception ex)
            {
                Respuesta.ok = false;
                Respuesta.mensaje = ex.Message;
            }

            return Respuesta;
        }
        public Respuesta CodeSend(Login _login)
        {
            Respuesta _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };

            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {

                    //set code
                    //JUAN CARLOS RODRIGUEZ DONAYRE 21-06-2021 (COMENTADO)
                    //code = Helpers.RandomString(4);
                    //FIN
                    var dominio = ConfigurationManager.AppSettings["DHost"];

                    //send email
                    var ml = new mailer();

                    //JUAN CARLOS RODRIGUEZ 28/05/2021 (COMENTADO)
                    //Boolean _result = ml.mail_tpl_reset_pass(_login.usuario_nombre, _login.Usuario_correoPersonal, _login.user, code, dominio);
                    //FIN

                    //Juan Carlos Rodríguez Donayre (20/10/2021)
                    string mail_Pwd = ConfigurationManager.AppSettings["pass"];
                    // Si el switch es 1 toma la contraseña desde la tabla de variables
                    string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                    if (switchReadPwdFromTable == "1")
                    {
                        Respuesta res = _conexion.consulta_Variables_Id(3);
                        mail_Pwd = res.mensaje;
                    }
                    //Fin

                    //JUAN CARLOS RODRIGUEZ 28/05/2021 (AGREGADO)
                    Boolean _result = false;
                    if (_login.tipoMensaje == "1")
                    {
                        _result = ml.mail_tpl_reset_pass(mail_Pwd, _login.usuario_nombre, _login.Usuario_correoPersonal, _login.code, dominio, DateTime.Today.Year.ToString());
                    }
                    if (_login.tipoMensaje == "2")
                    {
                        _result = ml.mail_confirm(mail_Pwd, _login.usuario_nombre, _login.Usuario_correoPersonal, _login.code, dominio, DateTime.Today.Year.ToString());
                    }
                    //FIN

                    if (_result == false)
                    {
                        throw new ApplicationException("No se envió el correo!!!");
                    }

                    _respuesta.ok = true;
                    _respuesta.mensaje = "";
                }

                if (_respuesta.ok == true)  //correo enviado OK, entonces inserta el code en SSO_code
                {
                    _respuesta = _conexion.RegisterCodeSend(_login);
                }

            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }

            return _respuesta;

        }
        public Respuesta CodeSendSMS(Login _login)
        {
            Respuesta _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {
                    //set code
                    //JUAN CARLOS RODRIGUEZ DONAYRE 21-06-2021 (COMENTADO)
                    //code = Helpers.RandomString(4);
                    //FIN

                    //Cambio de Proveedor SMS 12/10/2020 JCRodriguez-INI
                    var _API_ENVIO_SMS_NAME = ConfigurationManager.AppSettings["API_ENVIO_SMS_NAME"];
                    var _API_ENVIO_SMS_USR = ConfigurationManager.AppSettings["API_ENVIO_SMS_USR"];
                    var _API_ENVIO_SMS_PWD = ConfigurationManager.AppSettings["API_ENVIO_SMS_PWD"];
                    var _API_ENVIO_SENDER_ID = ConfigurationManager.AppSettings["API_ENVIO_SENDER_ID"];
                    //JUAN CARLOS RODRIGUEZ DONAYRE 28/05/2021 (COMENATDO)
                    //var _MENSAJE             = "HOLA " + _login.usuario_nombre.ToUpper() + ", TU CODIGO DE VERIFICACION DE MI CUENTA UPC ES: " + code;
                    //FIN

                    //Juan Carlos Rodríguez Donayre (20/10/2021)
                    // Si el switch es 1 toma la contraseña desde la tabla de variables
                    string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                    if (switchReadPwdFromTable == "1")
                    {
                        Respuesta res = _conexion.consulta_Variables_Id(4);
                        _API_ENVIO_SMS_PWD = res.mensaje;
                    }
                    //Fin

                    string _MENSAJE = "";
                    if (_login.tipoMensaje == "1")
                    {
                        _MENSAJE = "HOLA " + _login.usuario_nombre.ToUpper() + ", TU CODIGO DE VERIFICACION DE MI CUENTA UPC ES: " + _login.code;
                    }
                    if (_login.tipoMensaje == "2")
                    {
                        _MENSAJE = "HOLA " + _login.usuario_nombre.ToUpper() + ", TU CODIGO DE VERIFICACION DE ACTUALIZACION DE DATOS UPC ES: " + _login.code; 
                    }

                    var client = new RestClient(_API_ENVIO_SMS_NAME);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("usuario", _API_ENVIO_SMS_USR);
                    request.AddParameter("password", _API_ENVIO_SMS_PWD);
                    request.AddParameter("celular", _login.usuario_telefono);
                    request.AddParameter("mensaje", _MENSAJE);
                    request.AddParameter("senderId", _API_ENVIO_SENDER_ID);
                    IRestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);

                    var _rpta = JsonConvert.DeserializeObject<sms>(response.Content);
                    var _op = _rpta.estado;
                    if (_op == 1)
                    {
                        _respuesta.ok = true;
                        _respuesta.mensaje = "";
                    }
                    else
                    {
                        _respuesta.ok = false;
                        _respuesta.mensaje = "SMS no se envió!!!";
                    }

                }

                if (_respuesta.ok == true)  //SMS enviado OK, entonces inserta el code en SSO_code
                {
                    _respuesta = _conexion.CodeSendSMS(_login);
                }

            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta evaluaJWT(Login _login)
        {
            var respuesta = _conexion.evaluaJWT(_login);
            if (respuesta.ok)
            {
                Login _loginUsuario = new Login() { user = respuesta.obj.usuario_login };
                respuesta.obj = consultaAD(_loginUsuario);
            }

            return respuesta;
        }
        public Respuesta evaluaFace(Login _login)
        {
            var respuesta = _conexion.evaluaFace(_login);
            if (respuesta.ok)
            {
                Login _loginUsuario = new Login() { user = respuesta.obj.usuario_login };
                respuesta.obj = consultaAD(_loginUsuario);
            }

            return respuesta;
        }

        //JUAN CARLOS RODRIGUEZ DONAYRE 13-09-2021 (AGREGADO)
        public UsuarioAD Auth_Azure(Login login)
        {
            UsuarioAD UsuarioAD = new UsuarioAD();
            Respuesta Respuesta = new Respuesta();

            Respuesta = Api_Auth_A(login);
            if (Respuesta.code == 0)
            {
                UsuarioAD = Api_Datos_Usuario_A(login);
            }
            else
            {
                UsuarioAD.usuario_code = Respuesta.code;
                UsuarioAD.usuario_msg = Respuesta.mensaje;
            }
            return UsuarioAD;
        }
        public Respuesta Api_Auth_A(Login login)
        {
            Respuesta rpta = new Respuesta();
            try
            {
                var ApiDominioAuthA = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuthA"].ToString();
                var ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }
                //Fin

                Auth credenAuth = new Auth();
                credenAuth.alias = login.user;
                credenAuth.password = login.password;

                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(ApiDominioAuthA);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var request = clienteHTTP.PostAsync("/apisso/Api/SSO/Autenticacion", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var rptaAuth = JsonConvert.DeserializeObject<ERes>(resultString);

                    rpta.code = rptaAuth.code;
                    rpta.mensaje = MensajesAD(rptaAuth.code);
                }
                else
                {
                    throw new ApplicationException("API /apisso/Api/SSO/Autenticacion no devolvió información.");
                }
            }
            catch
            {
                rpta.code = 7;
                rpta.mensaje = MensajesAD(7);
            }
            return rpta;

        }
        public UsuarioAD Api_Datos_Usuario_A(Login login)
        {
            UsuarioAD usuarioAD = new UsuarioAD();
            try
            {
                int resultado = 0;

                var ApiDominioAuthA = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuthA"].ToString();
                var ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }
                //Fin

                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(ApiDominioAuthA);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                AuthAlias credenAuth = new AuthAlias();
                credenAuth.alias = login.user;
                var request = clienteHTTP.PostAsync("/apisso/Api/SSO/DatosUsuario", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var rptaAuth = JsonConvert.DeserializeObject<ERes>(resultString);
                    resultado = rptaAuth.code;
                    if (resultado == 0)
                    {
                        usuarioAD.usuario_login = login.user;
                        usuarioAD.usuario_nombre = string.IsNullOrEmpty(rptaAuth.AdUser.Nombres) ? "" : rptaAuth.AdUser.Nombres;
                        usuarioAD.usuario_apPaterno = string.IsNullOrEmpty(rptaAuth.AdUser.Apellidos) ? "" : rptaAuth.AdUser.Apellidos;
                        usuarioAD.usuario_apMaterno = "";
                        usuarioAD.Usuario_correoPersonal = string.IsNullOrEmpty(rptaAuth.AdUser.Email) ? "" : rptaAuth.AdUser.Email;
                        usuarioAD.usuario_correoUPC = string.IsNullOrEmpty(rptaAuth.AdUser.EmailUPC) ? "" : rptaAuth.AdUser.EmailUPC;
                        usuarioAD.usuario_telefono = string.IsNullOrEmpty(rptaAuth.AdUser.Anexo) ? "" : rptaAuth.AdUser.Anexo;
                        usuarioAD.usuario_code = 0;
                        usuarioAD.usuario_msg = MensajesAD(0);
                        usuarioAD.usuarioTipo = Api_Usuarios_Tipo(login.user);
                    }
                    else
                    {
                        throw new ApplicationException("API /apisso/Api/SSO/DatosUsuario no devolvió información.");
                    }
                }
                else
                {
                    throw new ApplicationException("API /apisso/Api/SSO/DatosUsuario no devolvió información.");
                }
            }
            catch
            {
                usuarioAD.usuario_code = 7;
                usuarioAD.usuario_msg = MensajesAD(7);
            }
            return usuarioAD;

        }

        public UsuarioAD Auth_OnPremises(Login login)
        {
            UsuarioAD UsuarioAD = new UsuarioAD();
            Respuesta Respuesta = new Respuesta();

            Respuesta = Api_Auth_O(login);
            if (Respuesta.code == 0)
            {
                UsuarioAD = Api_Datos_Usuario_O(login);
            }
            else
            {
                UsuarioAD.usuario_code = Respuesta.code;
                UsuarioAD.usuario_msg = Respuesta.mensaje;
            }
            return UsuarioAD;

        }
        public Respuesta Api_Auth_O(Login login)
        {
            Respuesta rpta = new Respuesta();
            try
            {
                var ApiDominioAuth = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
                var ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }
                //Fin

                Auth credenAuth = new Auth();
                credenAuth.alias = login.user;
                credenAuth.password = login.password;

                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(ApiDominioAuth);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var _request = clienteHTTP.PostAsync("/v3.0/SSO/Autenticacion", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (_request.IsSuccessStatusCode)
                {
                    var resultString = _request.Content.ReadAsStringAsync().Result;
                    var rptaAuth = JsonConvert.DeserializeObject<objClaims>(resultString);
                    rpta.code = Convert.ToInt16(rptaAuth.code);
                    rpta.mensaje = MensajesAD(Convert.ToInt16(rptaAuth.code));
                }
                else
                {
                    throw new ApplicationException("API /v3.0/SSO/Autenticacion no devolvió información.");
                }

            }
            catch
            {
                rpta.code = 7;
                rpta.mensaje = MensajesAD(7);
            }
            return rpta;

        }
        public UsuarioAD Api_Datos_Usuario_O(Login login)
        {
            UsuarioAD usuarioAD = new UsuarioAD();
            try
            {
                string resultado = "";

                var ApiDominioAuth = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
                var ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }
                //Fin

                AuthAlias credenAuth = new AuthAlias();
                credenAuth.alias = login.user;

                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(ApiDominioAuth);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var request = clienteHTTP.PostAsync("/v3.0/SSO/DatosUsuario", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var _rptaAuth = JsonConvert.DeserializeObject<objClaimsDatosUsuario>(resultString);
                    resultado = _rptaAuth.DTOHeader.CodigoRetorno;
                    if (resultado == "0")
                    {
                        usuarioAD.usuario_login = login.user;
                        usuarioAD.usuario_nombre = string.IsNullOrEmpty(_rptaAuth.DTOUsuario.Nombres) ? "" : _rptaAuth.DTOUsuario.Nombres;
                        usuarioAD.usuario_apPaterno = string.IsNullOrEmpty(_rptaAuth.DTOUsuario.Apellidos) ? "" : _rptaAuth.DTOUsuario.Apellidos;
                        usuarioAD.usuario_apMaterno = "";
                        usuarioAD.Usuario_correoPersonal = string.IsNullOrEmpty(_rptaAuth.DTOUsuario.Email) ? "" : _rptaAuth.DTOUsuario.Email;
                        usuarioAD.usuario_correoUPC = string.IsNullOrEmpty(_rptaAuth.DTOUsuario.EmailUPC) ? "" : _rptaAuth.DTOUsuario.EmailUPC;
                        usuarioAD.usuario_telefono = string.IsNullOrEmpty(_rptaAuth.DTOUsuario.Anexo) ? "" : _rptaAuth.DTOUsuario.Anexo;
                        usuarioAD.usuario_code = 0;
                        usuarioAD.usuario_msg = MensajesAD(0);
                        usuarioAD.usuarioTipo = Api_Usuarios_Tipo(login.user); 
                    }
                    else
                    {
                        throw new ApplicationException("API /v3.0/SSO/DatosUsuario no devolvió información.");
                    }
                }
                else
                {
                    throw new ApplicationException("API /v3.0/SSO/DatosUsuario no devolvió información.");
                }
            }
            catch
            {
                usuarioAD.usuario_code = 7;
                usuarioAD.usuario_msg = MensajesAD(7);
            }
            return usuarioAD;
        }

        public UsuarioAD Api_Usuarios(Login login)
        {
            UsuarioAD usuarioAD = new UsuarioAD();
            try
            {
                var ApiDominioAuth = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
                var ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }
                //Fin

                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(ApiDominioAuth);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                U_objClaims _objClaims = new U_objClaims();

                var request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=U&CodUsuario=" + login.user.ToUpper()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    _objClaims = JsonConvert.DeserializeObject<U_objClaims>(resultString);
                    if (_objClaims.ListaDTOUsuarios.Count > 0)
                    {
                        usuarioAD.usuario_login          = login.user;
                        usuarioAD.usuario_nombre         = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Nombres;
                        usuarioAD.usuario_apPaterno      = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].ApellidoPaterno;
                        usuarioAD.usuario_apMaterno      = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].ApellidoMaterno;
                        usuarioAD.Usuario_correoPersonal = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].EmailAlterno ?? "";
                        usuarioAD.usuario_correoUPC      = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Email ?? login.user + "@upc.edu.pe";
                        usuarioAD.usuario_telefono       = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].TelefonoMovil ?? "";
                        usuarioAD.usuario_code           = 0;
                        usuarioAD.usuario_msg            = MensajesAD(0);
                        usuarioAD.CodPersona             = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
                    }
                    else
                    {
                        request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=E&CodUsuario=" + login.user.ToUpper()).Result;
                        if (request.IsSuccessStatusCode)
                        {
                            var _resultString = request.Content.ReadAsStringAsync().Result;
                            _objClaims = JsonConvert.DeserializeObject<U_objClaims>(_resultString);
                            if (_objClaims.ListaDTOUsuarios.Count > 0)
                            {
                                usuarioAD.usuario_login          = login.user;
                                usuarioAD.usuario_nombre         = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Nombres;
                                usuarioAD.usuario_apPaterno      = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].ApellidoPaterno;
                                usuarioAD.usuario_apMaterno      = "";
                                usuarioAD.Usuario_correoPersonal = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].EmailAlterno ?? "";
                                usuarioAD.usuario_correoUPC      = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Email ?? login.user + "@upc.edu.pe";
                                usuarioAD.usuario_telefono       = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].TelefonoMovil ?? "";
                                usuarioAD.usuario_code           = 0;
                                usuarioAD.usuario_msg            = MensajesAD(0);
                                usuarioAD.CodPersona             = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
                            }
                            else
                            {
                                throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
                            }
                        }
                        else
                        {
                            throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
                        }
                    }

                    //Llama al api GET - /v3/crm/Contacto
                    if (!string.IsNullOrEmpty(usuarioAD.CodPersona))
                    {
                        var res = DataCRM(usuarioAD.CodPersona);
                        if (res.ok)
                        {
                            usuarioAD.usuario_telefono       = res.celular;
                            usuarioAD.Usuario_correoPersonal = res.correo;

                        }
                        else
                        {
                            throw new ApplicationException(res.mensaje);
                        }
                    }
                }
                else
                {
                    throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
                }
            }
            catch
            {
                usuarioAD.usuario_code = 7;
                usuarioAD.usuario_msg = MensajesAD(7);
            }
            return usuarioAD;
        }
        public class DataCRMget
        {
            public string TelefonoCelular { get; set; }
            public string CorreoPrincipal { get; set; }
        }
        public Respuesta DataCRM (string CodPersona)
        {

            Respuesta _respuesta = new Respuesta() {ok=false};

            try 
            {
                var _ApiDominio = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
                var _ApiCredencial = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    _ApiCredencial = res.mensaje;
                }
                //Fin

                //API - Busca el código de persona
                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(_ApiDominio);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + _ApiCredencial); // token generado en PostMan
                var request = clienteHTTP.GetAsync("/v3/crm/Contacto?TipoBusqueda=5&ValorBusqueda=" + CodPersona).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    DataCRMget _DataCRM = JsonConvert.DeserializeObject<DataCRMget>(resultString);

                    _respuesta.celular = _DataCRM.TelefonoCelular ?? "";
                    _respuesta.correo  = _DataCRM.CorreoPrincipal ?? "";
                    _respuesta.ok      = true;

                }
                else
                {
                    throw new ApplicationException("API /v3/crm/Contacto no devolvió información.");
                }
            }
            catch(Exception ex)
            {
                _respuesta.ok      = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;

        }

        public Respuesta ChangePasswordAD_API(Login _login)
        {
            Respuesta _respuesta = new Respuesta() { code = 0 };
            try
            {
                string _resultado = "";

                var _ApiDominioAuth = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
                var _ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    _ApiCredencialAuth = res.mensaje;
                }
                //Fin

                AuthChangePass _credenAuth = new AuthChangePass();
                _credenAuth.usuario = _login.user;
                _credenAuth.password = _login.password;

                HttpClient _clienteHTTP = new HttpClient();
                _clienteHTTP.BaseAddress = new Uri(_ApiDominioAuth);
                _clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + _ApiCredencialAuth); //token generado en PostMan

                var _request = _clienteHTTP.PostAsync("/v3.0/SSO/CambioContrasenia", _credenAuth, new JsonMediaTypeFormatter()).Result;
                if (_request.IsSuccessStatusCode)
                {
                    var resultString = _request.Content.ReadAsStringAsync().Result;
                    var _rptaAuth = JsonConvert.DeserializeObject<objClaims>(resultString);
                    _resultado = _rptaAuth.code;
                    _respuesta.ok = (_resultado == "0" ? true : false);
                    _respuesta.code = Convert.ToInt32(_resultado);
                    _respuesta.mensaje = MensajesAD(Convert.ToInt32(_resultado));
                }
                else
                {
                    throw new ApplicationException("CHPWD");
                }

            }
            catch
            {
                _respuesta.ok = false;
                _respuesta.code = 7;
                _respuesta.mensaje = MensajesAD(7);
            }
            return _respuesta;
        }
        //FIN

        //JUAN_CARLOS_RODRIGUEZ 18-03-2021 (agregado)
        public UsuarioAD ConsultaAD_Debug(Login _login)
        {
            List<UsuarioAD> _Lista = new List<UsuarioAD>()
            {
                 new UsuarioAD() {usuario_login = "JRODRIGUEZ",usuario_nombre = "JUAN CARLOS",usuario_apPaterno = "RODRIGUEZ DONAYRE",usuario_apMaterno = " ", Usuario_correoPersonal = "JUANCARLOS_DONAYRE@YAHOO.COM",usuario_correoUPC = "JC@UPC.COM.PE",      usuario_telefono = "950179960",usuario_code = 0,usuario_msg = MensajesAD(0)},
                 new UsuarioAD() {usuario_login = "LMENACHO",  usuario_nombre = "LUIS",       usuario_apPaterno = "MENACHO AGUIRRE",  usuario_apMaterno = " ", Usuario_correoPersonal = "luis.menacho@upc.pe",         usuario_correoUPC = "luis.menacho@upc.pe",usuario_telefono = "949605458",usuario_code = 0,usuario_msg = MensajesAD(0)}
            };
            UsuarioAD _usuarioAD = _Lista.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());

            return _usuarioAD;
        }

        public string Api_Usuarios_Tipo(string _user)
        {
            string usuarioTipo    = "";
            string Administrativo = "";
            string TipoPersona    = "";

            try
            {
                var ApiDominioAuth = System.Configuration.ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
                var ApiCredencialAuth = System.Configuration.ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

                //Juan Carlos Rodríguez Donayre (20/10/2021)
                // Si el switch es 1 toma la contraseña desde la tabla de variables
                string switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];
                if (switchReadPwdFromTable == "1")
                {
                    Respuesta res = _conexion.consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }
                //Fin

                HttpClient clienteHTTP = new HttpClient();
                clienteHTTP.BaseAddress = new Uri(ApiDominioAuth);
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                U_objClaims _objClaims = new U_objClaims();

                var request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=U&CodUsuario=" + _user.ToUpper()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    _objClaims = JsonConvert.DeserializeObject<U_objClaims>(resultString);
                    if (_objClaims.ListaDTOUsuarios.Count > 0)
                    {
                        Administrativo = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Administrativo;
                        TipoPersona    = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].TipoPersona;

                        //Alumno pregrado_epe
                        if (Administrativo == "NO" && TipoPersona == "ALU") { usuarioTipo = "PREGRADO_EPE"; }
                        //Administrativo
                        if (Administrativo == "SI") { usuarioTipo = "ADMINISTRATIVO"; }
                        //docente  
                        if (Administrativo == "NO" && TipoPersona == "PRF") { usuarioTipo = "DOCENTE"; }
                        //apoderado 
                        if (Administrativo == "NO" && TipoPersona == "PFA") { usuarioTipo = "APODERADO"; }
                    }
                    else
                    {
                        request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=E&CodUsuario=" + _user.ToUpper()).Result;
                        if (request.IsSuccessStatusCode)
                        {
                            var _resultString = request.Content.ReadAsStringAsync().Result;
                            _objClaims = JsonConvert.DeserializeObject<U_objClaims>(_resultString);
                            if (_objClaims.ListaDTOUsuarios.Count > 0)
                            {
                                Administrativo = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Administrativo;
                                TipoPersona = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].TipoPersona;

                                //Alumno epg
                                if (Administrativo == "NO" && TipoPersona == "ALU") { usuarioTipo = "EPG"; }
                                //docente  
                                if (Administrativo == "NO" && TipoPersona == "PRF") { usuarioTipo = "DOCENTE"; }
                                //apoderado 
                                if (Administrativo == "NO" && TipoPersona == "PFA") { usuarioTipo = "APODERADO"; }

                            }
                            else
                            {
                                throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
                            }
                        }
                        else
                        {
                            throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
                        }
                    }

                }
                else
                {
                    throw new ApplicationException("API /v2.1/Usuarios no devolvió información.");
                }
            }
            catch
            {
                usuarioTipo = "";
            }
            return usuarioTipo;
        }

    }
}
