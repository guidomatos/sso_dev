namespace SSO_BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using RestSharp;
    using SSO_BusinessLogic.Interfaces;
    using SSO_Modelo.Interfaces;
    using SSO_SecurityServerF;
    using SSO_SecurityServerF.Clases;
    using SSO_SecurityServerF.Mailer;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using SSO_Modelo.DTO;

    /// <summary>
    /// Clase para manejar metodos de autenticacion
    /// </summary>
    public class ProcessLogic : IProcessLogic
    {
        private readonly IConexion _conexion;
        private readonly string Cadena;
        private readonly string dominio;
        private string mail_Pwd;
        private readonly string switchReadPwdFromTable;
        private readonly string ApiDominioAuth;
        private readonly string ApiDominioAuthA;
        private string ApiCredencialAuth;
        private readonly string _API_ENVIO_SMS_NAME;
        private readonly string _API_ENVIO_SMS_USR;
        private string _API_ENVIO_SMS_PWD;
        private readonly string _API_ENVIO_SENDER_ID;

        /// <summary>
        /// Clase constructor
        /// </summary>
        /// <param name="conexion"></param>
        public ProcessLogic(IConexion conexion)
        {
            _conexion = conexion;
            Cadena = ConfigurationManager.AppSettings["switchAD"];
            dominio = ConfigurationManager.AppSettings["DHost"];
            mail_Pwd = ConfigurationManager.AppSettings["pass"];
            // Si el switch es 1 toma la contraseña desde la tabla de variables
            switchReadPwdFromTable = ConfigurationManager.AppSettings["switchReadPwdFromTable"];

            ApiDominioAuth = ConfigurationManager.AppSettings["ApiDominioAuth"].ToString();
            ApiDominioAuthA = ConfigurationManager.AppSettings["ApiDominioAuthA"].ToString();
            ApiCredencialAuth = ConfigurationManager.AppSettings["ApiCredencialAuth"].ToString();

            //Cambio de Proveedor SMS
            _API_ENVIO_SMS_NAME = ConfigurationManager.AppSettings["API_ENVIO_SMS_NAME"];
            _API_ENVIO_SMS_USR = ConfigurationManager.AppSettings["API_ENVIO_SMS_USR"];
            _API_ENVIO_SMS_PWD = ConfigurationManager.AppSettings["API_ENVIO_SMS_PWD"];
            _API_ENVIO_SENDER_ID = ConfigurationManager.AppSettings["API_ENVIO_SENDER_ID"];
        }

        /// <summary>
        /// Consulta datos de usuario del Active Directory
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public UsuarioAD ConsultaAD(Login _login)
        {
            var _usuarioAD = new UsuarioAD();
            var _AD = new AD();
            bool _result;
            if (!string.IsNullOrEmpty(_login.password))
            {
                var r = _AD.Auth(_login.user, _login.password);
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
                var userInfo = _AD.GetUser(_login.user);
                _result = userInfo.Result.ok;
                _usuarioAD.usuario_code = userInfo.Result.code;
                _usuarioAD.usuario_msg = MensajesAD(userInfo.Result.code);

                if (_result)
                {
                    _usuarioAD.usuario_login = _login.user;
                    _usuarioAD.usuario_nombre = userInfo.Result.AdUser.Name;
                    _usuarioAD.usuario_apPaterno = userInfo.Result.AdUser.LastName;
                    _usuarioAD.usuario_apMaterno = " ";
                    _usuarioAD.Usuario_correoPersonal = userInfo.Result.AdUser.Email;
                    _usuarioAD.usuario_correoUPC = userInfo.Result.AdUser.EmailUPC;
                    _usuarioAD.usuario_telefono = userInfo.Result.AdUser.Phone;
                    _usuarioAD.usuario_code = userInfo.Result.code;
                    _usuarioAD.usuario_msg = MensajesAD(userInfo.Result.code);
                }
            }
            return _usuarioAD;
        }
        /// <summary>
        /// Cambiar contraseña de usuario
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta ChangePasswordAD(Login _login)
        {
            Respuesta _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user) && !string.IsNullOrEmpty(_login.password))
                {
                    //var _AD = new AD();
                    //var _res = _AD.ChangePasswordAD(_login.user, _login.password);
                    //_respuesta.ok = _res.Item1;
                    //_respuesta.mensaje = _res.Item2;

                    // USO DE APIS Y LECTURA DE FEDERA DESDE BD (agregado)
                    //Respuesta _rpta = new Respuesta();
                    //_rpta = _conexion.Consulta_federada(_login.CodFederada);
                    //if (_rpta.federada_api == "A")
                    //{
                    //    _respuesta = ChangePasswordAD_API_A(_login);
                    //}
                    //else if (_rpta.federada_api == "O")
                    //{
                    //    _respuesta = ChangePasswordAD_API(_login);
                    //}

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

                    _respuesta = ChangePasswordAD_API(_login);

                }
                //REGISTRA FLAG A FALSO PARA NO RECUPERAR CONTRASEÑA AL INICIO SIGUIENTE
                if (_respuesta.ok == true)
                {
                    var _result = _conexion.RegistraFlag(_login);
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        /// <summary>
        /// Validar credenciales (usuario y contraseña)
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta ValidaCredenciales(Login _login)
        {
            var Respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user) && !string.IsNullOrEmpty(_login.password))
                {
                    //UsuarioAD _UsuarioAD = consultaAD(_login);
                    //USO DE APIS Y LECTURA DE FEDERA DESDE BD
                    var UsuarioAD = new UsuarioAD();
                    var rpta = _conexion.Consulta_federada(_login.CodFederada);
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

                    //SI EL INTENTO DE CONEXION AL AD DE AZURE FALLÓ Y EL CODIGO DE FEDERADA SE ENCUENTRA EN CADENA
                    //ENTONCES SE LOGUEA CON EL AD ONPREMISES
                    bool found = false;
                    string[] A = Cadena.Split(',');
                    for (int i = 0; i <= A.Length - 1; i++)
                        if (A[i] == _login.CodFederada)
                            found = true;
                    if ((rpta.federada_api == "A" && UsuarioAD.usuario_code != 0) && found)
                        UsuarioAD = Auth_OnPremises(_login);

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

                if (Respuesta.ok)  //credenciales correctas, actualiza tablas involucradas en el login
                {
                    //ENVIO CODIGO DE FEDERADA
                    Respuesta.CodFederada = _login.CodFederada;

                    Respuesta = _conexion.RegistrarValidaCredenciales(Respuesta);
                }

                //CONSULTA FLAG RECUPERAR CONTRASEÑA
                if (Respuesta.ok)
                {
                    Respuesta.flagRecPas = _conexion.ConsultaFlag(_login);
                }
            }
            catch
            {
                throw;
            }

            return Respuesta;
        }
        /// <summary>
        /// Validar credenciales (solo usuario)
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta ValidaSoloUsuario(Login _login)
        {
            var Respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {
                    //UsuarioAD UsuarioAD  = consultaAD(_login);

                    var UsuarioAD = Api_Usuarios(_login);
                    //var UsuarioAD = MockGetUser();

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
                if (Respuesta.ok)  //credenciales correctas, actualiza tablas involucradas en el login
                {
                    //ENVIO CODIGO DE FEDERADA
                    Respuesta.CodFederada = _login.CodFederada;

                    Respuesta = _conexion.RegistrarValidaCredenciales(Respuesta);
                }
            }
            catch
            {
                throw;
            }

            return Respuesta;
        }
        /// <summary>
        /// Enviar correo electronico
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta CodeSend(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };

            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {

                    //code = Helpers.RandomString(4);

                    //send email
                    var ml = new mailer();

                    //Boolean _result = ml.mail_tpl_reset_pass(_login.usuario_nombre, _login.Usuario_correoPersonal, _login.user, code, dominio);

                    // Si el switch es 1 toma la contraseña desde la tabla de variables
                    if (switchReadPwdFromTable == "1")
                    {
                        var res = _conexion.Consulta_Variables_Id(3);
                        mail_Pwd = res.mensaje;
                    }

                    bool _result = false;
                    if (_login.tipoMensaje == "1")
                    {
                        _result = ml.mail_tpl_reset_pass(mail_Pwd, _login.usuario_nombre, _login.Usuario_correoPersonal, _login.code, dominio, DateTime.Today.Year.ToString());
                    }
                    if (_login.tipoMensaje == "2")
                    {
                        _result = ml.mail_confirm(mail_Pwd, _login.usuario_nombre, _login.Usuario_correoPersonal, _login.code, dominio, DateTime.Today.Year.ToString());
                    }

                    if (!_result)
                    {
                        throw new ApplicationException("No se envió el correo!!!");
                    }

                    _respuesta.ok = true;
                    _respuesta.mensaje = "";
                }

                if (_respuesta.ok)  //correo enviado OK, entonces inserta el code en SSO_code
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
        /// <summary>
        /// Enviar SMS
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta CodeSendSMS(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {
                    //code = Helpers.RandomString(4);

                    //var _MENSAJE = "HOLA " + _login.usuario_nombre.ToUpper() + ", TU CODIGO DE VERIFICACION DE MI CUENTA UPC ES: " + code;

                    // Si el switch es 1 toma la contraseña desde la tabla de variables
                    if (switchReadPwdFromTable == "1")
                    {
                        var res = _conexion.Consulta_Variables_Id(4);
                        _API_ENVIO_SMS_PWD = res.mensaje;
                    }

                    string _MENSAJE = "";
                    if (_login.tipoMensaje == "1")
                    {
                        _MENSAJE = "HOLA " + _login.usuario_nombre.ToUpper() + ", TU CODIGO DE VERIFICACION DE MI CUENTA UPC ES: " + _login.code;
                    }
                    if (_login.tipoMensaje == "2")
                    {
                        _MENSAJE = "HOLA " + _login.usuario_nombre.ToUpper() + ", TU CODIGO DE VERIFICACION DE ACTUALIZACION DE DATOS UPC ES: " + _login.code;
                    }

                    var client = new RestClient(_API_ENVIO_SMS_NAME)
                    {
                        Timeout = -1
                    };
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("usuario", _API_ENVIO_SMS_USR);
                    request.AddParameter("password", _API_ENVIO_SMS_PWD);
                    request.AddParameter("celular", _login.usuario_telefono);
                    request.AddParameter("mensaje", _MENSAJE);
                    request.AddParameter("senderId", _API_ENVIO_SENDER_ID);
                    IRestResponse response = client.Execute(request);
                    //Console.WriteLine(response.Content);

                    var _rpta = JsonConvert.DeserializeObject<SendSmsResponseDto>(response.Content);
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

                if (_respuesta.ok)  //SMS enviado OK, entonces inserta el code en SSO_code
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
        /// <summary>
        /// Valida token y de estar ok devuelve datos de usuario
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta EvaluaJWT(Login _login)
        {
            var respuesta = _conexion.EvaluaJWT(_login);
            if (respuesta.ok)
            {
                var _loginUsuario = new Login() { user = respuesta.obj.usuario_login };
                respuesta.obj = ConsultaAD(_loginUsuario);
            }

            return respuesta;
        }
        /// <summary>
        /// Validar email facebook y de estar ok devuelve datos de usuario
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public Respuesta EvaluaFace(Login _login)
        {
            var respuesta = _conexion.EvaluaFace(_login);
            if (respuesta.ok)
            {
                var _loginUsuario = new Login() { user = respuesta.obj.usuario_login };
                respuesta.obj = ConsultaAD(_loginUsuario);
            }

            return respuesta;
        }

        #region "Metodos privados"
        /// <summary>
        /// Validar autenticacion de usuario y de estar ok devolver datos de usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private UsuarioAD Auth_Azure(Login login)
        {
            var UsuarioAD = new UsuarioAD();
            var Respuesta = Api_Auth_A(login);
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
        /// <summary>
        /// Validar autenticacion de usuario y devolver estado de respuesta
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private Respuesta Api_Auth_A(Login login)
        {
            var rpta = new Respuesta();
            try
            {

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var credenAuth = new AuthenticationOnAzureRequestDto
                {
                    alias = login.user,
                    password = login.password
                };

                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuthA)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var request = clienteHTTP.PostAsync("/apisso/Api/SSO/Autenticacion", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var rptaAuth = JsonConvert.DeserializeObject<AuthenticationOnAzureResponseDto>(resultString);

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
        /// <summary>
        /// Validar autenticacion de usuario y devolver datos de usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private UsuarioAD Api_Datos_Usuario_A(Login login)
        {
            var usuarioAD = new UsuarioAD();
            try
            {
                int resultado = 0;


                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuthA)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var credenAuth = new GetUserInfoFromAzureRequestDto
                {
                    alias = login.user
                };
                var request = clienteHTTP.PostAsync("/apisso/Api/SSO/DatosUsuario", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var rptaAuth = JsonConvert.DeserializeObject<GetUserInfoFromAzureResponseDto>(resultString);
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
        /// <summary>
        /// Validar autenticacion de usuario (On Premise) y devolver datos de usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private UsuarioAD Auth_OnPremises(Login login)
        {
            var UsuarioAD = new UsuarioAD();
            var Respuesta = Api_Auth_O(login);
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
        /// <summary>
        /// Validar autenticacion de usuario (On Premise) y devolver estado de respuesta
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private Respuesta Api_Auth_O(Login login)
        {
            var rpta = new Respuesta();
            try
            {

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var credenAuth = new AuthenticationOnPremiseRequestDto
                {
                    alias = login.user,
                    password = login.password
                };

                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuth)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var _request = clienteHTTP.PostAsync("/v3.0/SSO/Autenticacion", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (_request.IsSuccessStatusCode)
                {
                    var resultString = _request.Content.ReadAsStringAsync().Result;
                    var rptaAuth = JsonConvert.DeserializeObject<AuthenticationOnPremiseResponseDto>(resultString);
                    rpta.code = Convert.ToInt32(rptaAuth.code);
                    rpta.mensaje = MensajesAD(Convert.ToInt32(rptaAuth.code));
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
        /// <summary>
        /// Validar autenticacion de usuario (On Premise) y devolver datos de usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private UsuarioAD Api_Datos_Usuario_O(Login login)
        {
            var usuarioAD = new UsuarioAD();
            try
            {
                string resultado = "";

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var credenAuth = new GetUserInfoFromOnPremiseRequestDto
                {
                    alias = login.user
                };

                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuth)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var request = clienteHTTP.PostAsync("/v3.0/SSO/DatosUsuario", credenAuth, new JsonMediaTypeFormatter()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var _rptaAuth = JsonConvert.DeserializeObject<GetUserInfoFromOnPremiseResponseDto>(resultString);
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
        /// <summary>
        /// Devolver datos de usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private UsuarioAD Api_Usuarios(Login login)
        {
            var usuarioAD = new UsuarioAD();
            try
            {

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuth)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var _objClaims = new GetUserInfoResponseDto();

                var request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=U&CodUsuario=" + login.user.ToUpper()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    _objClaims = JsonConvert.DeserializeObject<GetUserInfoResponseDto>(resultString);
                    if (_objClaims.ListaDTOUsuarios.Count > 0)
                    {
                        MappingUserAD(usuarioAD, _objClaims, login.user);
                    }
                    else
                    {
                        request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=E&CodUsuario=" + login.user.ToUpper()).Result;
                        if (request.IsSuccessStatusCode)
                        {
                            var _resultString = request.Content.ReadAsStringAsync().Result;
                            _objClaims = JsonConvert.DeserializeObject<GetUserInfoResponseDto>(_resultString);
                            if (_objClaims.ListaDTOUsuarios.Count > 0)
                            {
                                MappingUserAD(usuarioAD, _objClaims, login.user);
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
                            usuarioAD.usuario_telefono = res.celular;
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
                throw;
            }
            return usuarioAD;
        }
        /// <summary>
        /// Devolver estado de respuesta
        /// </summary>
        /// <param name="CodPersona"></param>
        /// <returns></returns>
        private Respuesta DataCRM(string CodPersona)
        {

            var _respuesta = new Respuesta() { ok = false };

            try
            {

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                //API - Busca el código de persona
                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuth)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); // token generado en PostMan
                var request = clienteHTTP.GetAsync("/v3/crm/Contacto?TipoBusqueda=5&ValorBusqueda=" + CodPersona).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    var _DataCRM = JsonConvert.DeserializeObject<GetContactInfoResponseDto>(resultString);

                    _respuesta.celular = _DataCRM.TelefonoCelular ?? "";
                    _respuesta.correo = _DataCRM.CorreoPrincipal ?? "";
                    _respuesta.ok = true;

                }
                else
                {
                    throw new ApplicationException("API /v3/crm/Contacto no devolvió información.");
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;

        }
        /// <summary>
        /// Mapear datos de usuario
        /// </summary>
        /// <param name="destinationDto"></param>
        /// <param name="originDto"></param>
        /// <param name="loginUser"></param>
        private void MappingUserAD(UsuarioAD destinationDto, GetUserInfoResponseDto originDto, string loginUser)
        {
            destinationDto.usuario_login = loginUser;
            destinationDto.usuario_nombre = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Nombres;
            destinationDto.usuario_apPaterno = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].ApellidoPaterno;
            destinationDto.usuario_apMaterno = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].ApellidoMaterno ?? "";
            destinationDto.Usuario_correoPersonal = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].EmailAlterno ?? "";
            destinationDto.usuario_correoUPC = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Email ?? loginUser + "@upc.edu.pe";
            destinationDto.usuario_telefono = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].TelefonoMovil ?? "";
            destinationDto.usuario_code = 0;
            destinationDto.usuario_msg = MensajesAD(0);
            destinationDto.CodPersona = originDto.ListaDTOUsuarios[0].DetalleDTOUsuario[0].CodPersona;
        }
        /// <summary>
        /// Invocacion a API para cambio de contraseña
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        private Respuesta ChangePasswordAD_API(Login _login)
        {
            var _respuesta = new Respuesta() { code = 0 };
            try
            {
                string _resultado = "";

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var _credenAuth = new ChangePasswordRequestDto
                {
                    usuario = _login.user,
                    password = _login.password
                };

                var _clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuth)
                };
                _clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var _request = _clienteHTTP.PostAsync("/v3.0/SSO/CambioContrasenia", _credenAuth, new JsonMediaTypeFormatter()).Result;
                if (_request.IsSuccessStatusCode)
                {
                    var resultString = _request.Content.ReadAsStringAsync().Result;
                    var _rptaAuth = JsonConvert.DeserializeObject<ChangePasswordResponseDto>(resultString);
                    _resultado = _rptaAuth.code;
                    _respuesta.ok = (_resultado == "0");
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
        /// <summary>
        /// Validar datos de usuario con mock de BD
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        private UsuarioAD ConsultaAD_Debug(Login _login)
        {
            var _Lista = new List<UsuarioAD>()
            {
                 new UsuarioAD() {usuario_login = "JRODRIGUEZ",usuario_nombre = "JUAN CARLOS",usuario_apPaterno = "RODRIGUEZ DONAYRE",usuario_apMaterno = " ", Usuario_correoPersonal = "JUANCARLOS_DONAYRE@YAHOO.COM",usuario_correoUPC = "JC@UPC.COM.PE",      usuario_telefono = "950179960",usuario_code = 0,usuario_msg = MensajesAD(0)},
                 new UsuarioAD() {usuario_login = "LMENACHO",  usuario_nombre = "LUIS",       usuario_apPaterno = "MENACHO AGUIRRE",  usuario_apMaterno = " ", Usuario_correoPersonal = "luis.menacho@upc.pe",         usuario_correoUPC = "luis.menacho@upc.pe",usuario_telefono = "949605458",usuario_code = 0,usuario_msg = MensajesAD(0)}
            };
            var _usuarioAD = _Lista.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());

            return _usuarioAD;
        }
        /// <summary>
        /// Devolver tipo de usuario
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        private string Api_Usuarios_Tipo(string _user)
        {
            string usuarioTipo = "";
            try
            {

                // Si el switch es 1 toma la contraseña desde la tabla de variables
                if (switchReadPwdFromTable == "1")
                {
                    var res = _conexion.Consulta_Variables_Id(5);
                    ApiCredencialAuth = res.mensaje;
                }

                var clienteHTTP = new HttpClient
                {
                    BaseAddress = new Uri(ApiDominioAuth)
                };
                clienteHTTP.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiCredencialAuth); //token generado en PostMan

                var _objClaims = new GetUserInfoResponseDto();

                var request = clienteHTTP.GetAsync("/v2.1/Usuarios?CodLineaNegocio=U&CodUsuario=" + _user.ToUpper()).Result;
                if (request.IsSuccessStatusCode)
                {
                    var resultString = request.Content.ReadAsStringAsync().Result;
                    _objClaims = JsonConvert.DeserializeObject<GetUserInfoResponseDto>(resultString);
                    string Administrativo;
                    string TipoPersona;
                    if (_objClaims.ListaDTOUsuarios.Count > 0)
                    {
                        Administrativo = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].Administrativo;
                        TipoPersona = _objClaims.ListaDTOUsuarios[0].DetalleDTOUsuario[0].TipoPersona;

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
                            _objClaims = JsonConvert.DeserializeObject<GetUserInfoResponseDto>(_resultString);
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
        /// <summary>
        /// Devolver mensaje segun codigo
        /// </summary>
        /// <param name="_code"></param>
        /// <returns></returns>
        private string MensajesAD(int _code)
        {
            string Observa;
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
        #endregion

        #region "Mock"

        private UsuarioAD MockGetUser()
        {
            var user = new UsuarioAD
            {
                usuario_login = "eguillen",
                usuario_nombre = "Eduardo",
                usuario_apPaterno = "Guillén",
                usuario_apMaterno = "",
                Usuario_correoPersonal = "eguillen@pruebas.com",
                usuario_correoUPC = "eguillen@upc.edu.pe",
                usuario_telefono = "9556865832",
                usuario_code = 0,
                usuario_msg = MensajesAD(0),
                CodPersona = ""
            };

            return user;
        }

        #endregion
    }
}
