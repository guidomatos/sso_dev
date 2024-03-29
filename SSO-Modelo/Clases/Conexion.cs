﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using SSO_Modelo.Interfaces;
using SSO_Modelo.Modelo;
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;


namespace SSO_Modelo.Clases
{
    /// <summary>
    /// 
    /// </summary>
    public class Conexion : IConexion
    {
        private readonly IEncriptador _encriptador;
        private readonly SSOEntities BD;
        public Conexion
        (
            IEncriptador encriptador,
            SSOEntities _BD
        )
        {
            _encriptador = encriptador;
            BD = _BD;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public bool RegistraUltActDatos(Login _login)
        {
            var _result = false;
            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());
            _reg.usuario_ultActDatos = DateTime.Now;
            _result = BD.SaveChanges() > 0;
            return _result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_login"></param>
        /// <returns></returns>
        public bool RegistraUsuarioTipo(Login _login) // Solo para el proceso de carga del campo usuario_tipo
        {
            var _result = false;
            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());
            _reg.usuario_tipo = _login.usuarioTipo;
            _result = BD.SaveChanges() > 0;
            return _result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SSO_usuario> ListaDeUsuarios()       // Solo para el proceso de carga del campo usuario_tipo
        {
            //var Listado = BD.SSO_usuario.ToList();
            //return Listado;

            var Listado = (from u in BD.SSO_usuario
                           where u.usuario_tipo == null
                           select u).ToList();
            return Listado;
        }
        public bool RegistraFlag(Login _login)
        {
            var _result = false;
            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());
            _reg.usuario_flagRecPas = false;
            _result = BD.SaveChanges() > 0;
            return _result;
        }
        public bool ConsultaFlag(Login _login)
        {
            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());
            var _flag = (bool)_reg.usuario_flagRecPas;
            return _flag;
        }
        public Respuesta Consulta_SSO_usuarioRed(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());

                var _reg_consulta = BD.SSO_usuarioRed.FirstOrDefault(x => x.usuarioRed_usuario_id == _reg.usuario_id
                                                                     && x.usuarioRed_redSocial_id == _login.redSocial_id);
                var _reg_claims = new JwtClaims();

                if (!(_reg_consulta == null))
                {
                    //_reg_claims.email = _reg_consulta.usuarioRed_correo_usuario;
                    _reg_claims.name = _reg_consulta.usuarioRed_nombre_usuario;
                    _reg_claims.picture = _reg_consulta.usuarioRed_imagenUrl;

                    _respuesta.ok = true;
                    _respuesta.objJwtClaims = _reg_claims;
                }

            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta Consulta_NoMostrar(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {
                    var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());
                    _respuesta.ok = true;
                    _respuesta.noMostrar = _reg.usuario_flagAsociarRedes;
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta Consulta_Variables(string nombreVariable)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                if (!string.IsNullOrEmpty(nombreVariable))
                {
                    var _reg = BD.SSO_variable.FirstOrDefault(x => x.variable_nombre.ToUpper() == nombreVariable.ToUpper());
                    _respuesta.ok = true;
                    _respuesta.mensaje = _reg.variable_valor;
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta Consulta_Variables_Id(int variableId)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                if (variableId > 0)
                {
                    var _reg = BD.SSO_variable.FirstOrDefault(x => x.variable_id == variableId);
                    _respuesta.ok = true;
                    _respuesta.mensaje = _reg.variable_valor;
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta RegistrarValidaCredenciales(Respuesta _respuesta)
        {
            _respuesta.ok = false;
            _respuesta.mensaje = "Sesión expirada";
            try
            {
                bool _result = false;

                _result = RegistraEn_SSO_usuario(_respuesta.obj);
                if (_result == false) throw new ApplicationException("No registró en SSO_usuario");

                _result = RegistraEn_SSO_usuarioCnx(_respuesta.obj, _respuesta.CodFederada);
                if (_result == false) throw new ApplicationException("No regitró en SSO_usuarioCnx");

                _respuesta.ok = true;
                _respuesta.mensaje = "";
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }

            return _respuesta;
        }
        public Respuesta RegisterCodeSend(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                var _result = false;
                var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login == _login.user);
                SSO_code _sso_code = new SSO_code()
                {
                    code_usuario_id = _reg.usuario_id,
                    code_CodeValue = _login.code,
                    code_fechaRegistro = DateTime.Now,
                    code_medioEnvio = "MAIL"
                };
                _result = RegistraEn_SSO_code(_sso_code, _login);
                if (!_result) throw new ApplicationException("No registró en SSO_code");

                _respuesta.ok = true;
                _respuesta.mensaje = "";
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
            var _respuesta = new Respuesta() { ok = false, mensaje = "!! Error: Usuario incorrecto !!" };
            try
            {
                var _result = false;
                var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login == _login.user);
                var _sso_code = new SSO_code()
                {
                    code_usuario_id = _reg.usuario_id,
                    code_CodeValue = _login.code,
                    code_fechaRegistro = DateTime.Now,
                    code_medioEnvio = "SMS"
                };
                _result = RegistraEn_SSO_code(_sso_code, _login);
                if (!_result) throw new ApplicationException("No registró en SSO_code");

                _respuesta.ok = true;
                _respuesta.mensaje = "";
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta ProcesaJWT(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };
            var _claims = new JwtClaims();
            try
            {
                if (!string.IsNullOrEmpty(_login.redSocial_token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(_login.redSocial_token) as JwtSecurityToken;

                    _claims.name = tokenS.Claims.First(claim => claim.Type == "name").Value;
                    _claims.email = tokenS.Claims.First(claim => claim.Type == "email").Value;
                    _claims.picture = tokenS.Claims.First(claim => claim.Type == "picture").Value;

                    _respuesta.ok = true;
                    _respuesta.objJwtClaims = _claims;
                }

                if (_respuesta.ok)
                {
                    var _result = RegistraEn_SSO_usuarioRed(_login, _respuesta);
                    if (!_result)
                    {
                        throw new ApplicationException("No se registró en SSO_usuarioRed");
                    }
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta UpdNoMostrar(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                if (!string.IsNullOrEmpty(_login.user))
                {
                    var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());
                    _reg.usuario_flagAsociarRedes = _login.noMostrar;
                    BD.SaveChanges();
                    _respuesta.ok = true;
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta EvaluaJWT(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada", obj = new UsuarioAD() };

            try
            {
                if (!string.IsNullOrEmpty(_login.redSocial_token))
                {
                    //Lee token
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(_login.redSocial_token) as JwtSecurityToken;
                    //Lee claims del token
                    var email = tokenS.Claims.First(claim => claim.Type == "email").Value;

                    if (!(email == null))
                    {
                        var _reg = BD.SSO_usuarioRed.FirstOrDefault(x => x.usuarioRed_correo_usuario.ToUpper() == email.ToUpper() && x.usuarioRed_redSocial_id == 2);
                        if (!(_reg == null))
                        {
                            var _regUsuario = BD.SSO_usuario.FirstOrDefault(x => x.usuario_id == _reg.usuarioRed_usuario_id);
                            _respuesta.ok = true;
                            _respuesta.obj.usuario_login = _regUsuario.usuario_login;
                        }
                        else
                        {
                            _respuesta.mensaje = "Cuenta de Google no vinculada";
                        }
                    }
                    else
                    {
                        _respuesta.mensaje = "Cuenta de Google no vinculada";
                    }
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta CodeEval(Login _login)
        {
            var _respuesta = new Respuesta
            {
                ok = false,
                mensaje = "Sesión expirada"
            };
            try
            {
                if (!(string.IsNullOrEmpty(_login.user) || _login.code == ""))
                {
                    var FechaHora = DateTime.Now.AddMinutes(-5);

                    var _reg = new SSO_code();
                    try
                    {
                        var _obj = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login == _login.user);
                        _reg = BD.SSO_code.OrderByDescending(x => x.code_fechaRegistro).FirstOrDefault(x => x.code_usuario_id == _obj.usuario_id && x.code_fechaRegistro >= FechaHora);

                        //_reg = ( from u in BD.SSO_usuario
                        //          join c in BD.SSO_code on u.usuario_id equals c.code_usuario_id
                        //          where u.usuario_login.ToUpper() == _login.user.ToUpper() && c.code_fechaRegistro >= FechaHora
                        //          select c ).Single();

                    }
                    catch
                    {
                        _reg = null;
                    }
                    if (_reg == null)
                    {
                        throw new ApplicationException("Código incorrecto");
                    }
                    if (!(_login.code == _reg.code_CodeValue))
                    {
                        throw new ApplicationException("Código incorrecto");
                    }
                    _respuesta.ok = true;
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta ProcesaFace(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };
            var _claims = new JwtClaims();
            try
            {
                if (!string.IsNullOrEmpty(_login.redSocial_name))
                {
                    _claims.name = _login.redSocial_name;
                    _claims.email = _login.redSocial_email;
                    _claims.picture = _login.redSocial_picture;

                    _respuesta.ok = true;
                    _respuesta.mensaje = "";
                    _respuesta.objJwtClaims = _claims;
                }

                if (_respuesta.ok)
                {
                    var _result = RegistraEn_SSO_usuarioRed(_login, _respuesta);
                    if (!_result)
                    {
                        throw new ApplicationException("No se registró en SSO_usuarioRed");
                    }
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta EvaluaFace(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada", obj = new UsuarioAD() };

            try
            {
                if (!(_login.redSocial_email == null))
                {
                    var _reg = BD.SSO_usuarioRed.FirstOrDefault(x => x.usuarioRed_correo_usuario.ToUpper() == _login.redSocial_email.ToUpper() && x.usuarioRed_redSocial_id == 1);
                    if (!(_reg == null))
                    {
                        // aqui debe generar un token de acceso
                        var _regUsuario = BD.SSO_usuario.FirstOrDefault(x => x.usuario_id == _reg.usuarioRed_usuario_id);
                        _respuesta.ok = true;
                        _respuesta.mensaje = "";
                        _respuesta.obj.usuario_login = _regUsuario.usuario_login;
                    }
                    else
                    {
                        _respuesta.mensaje = "Cuenta de Facebook no vinculada";
                    }
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta BorraEn_SSO_usuarioRed(Login _login)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };
            var _result = false;
            try
            {
                var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());

                var _reg_delete = BD.SSO_usuarioRed.FirstOrDefault(x => x.usuarioRed_usuario_id == _reg.usuario_id
                                                                        && x.usuarioRed_redSocial_id == _login.redSocial_id);
                if (!(_reg_delete == null))
                {
                    BD.SSO_usuarioRed.Remove(_reg_delete);
                    _result = BD.SaveChanges() > 0;

                    //Registra auditoria
                    if (_result) _result = RegistraEn_SSO_auditoria(_login.user.ToUpper(), "SSO_USUARIORED", "D");

                    if (_result)
                    {
                        _respuesta.ok = true;
                        _respuesta.mensaje = "";
                    }
                    else
                    {
                        throw new ApplicationException("No se pudo eliminar en SSO_usuarioRed");
                    }
                }
                else
                {
                    throw new ApplicationException("No se encontró dato en SSO_usuarioRed");
                }
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public string MensajesAD(int _code)
        {
            string Observa;
            switch (_code)
            {
                case 0:
                    Observa = "ConexiÃ³n establecida exitosamente.";
                    break;
                case 1:
                    Observa = "La cuenta de usuario ha sido bloqueda por exceder el nÃºmero de intentos fallidos. La cuenta serÃ¡ desbloqueada automÃ¡ticamente dentro de 30 minutos..";
                    break;
                case 2:
                    Observa = "El usuario ingresado se encuentra Inactivo.";
                    break;
                case 3:
                    Observa = "El usuario o contraseÃ±a es incorrecto, intÃ©ntelo nuevamente. Recuerde que luego de algunos intentos fallidos su cuenta se bloquearÃ¡.";
                    break;
                case 4:
                    Observa = "ConexiÃ³n no establecida. Es necesario que cambie su contraseÃ±a desde la opciÃ³n 'Olvidaste tu Clave'.";
                    break;
                case 5:
                    Observa = "ConexiÃ³n no establecida. Su contraseÃ±a ha caducado, por favor notifique a Help Desk.";
                    break;
                case 6:
                    Observa = "El usuario ingresado no existe. De persistir el problema, por favor notifique a Help Desk.";
                    break;
                default:
                    Observa = "OcurriÃ³ un error inesperado, intÃ©ntelo nuevamente.";
                    break;
            }
            return Observa;
        }
        public Respuesta Consulta_federada(string _codFederada)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                //if (!string.IsNullOrEmpty(_codFederada))
                //{
                var _reg = BD.SSO_federada.FirstOrDefault(x => x.federada_codigo == _codFederada);
                if (_reg == null)
                {
                    _reg = BD.SSO_federada.FirstOrDefault(x => x.federada_codigo == "Default");
                }
                _respuesta.ok = true;
                _respuesta.mensaje = "";
                _respuesta.federada = _reg.federada_url;
                _respuesta.federada_api = _reg.federada_api;
                //}
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public Respuesta Consulta_federada_nom(string _nomFederada)
        {
            var _respuesta = new Respuesta() { ok = false, mensaje = "Sesión expirada" };

            try
            {
                var _reg = BD.SSO_federada.FirstOrDefault(x => x.federada_nombre.ToUpper() == _nomFederada.ToUpper());
                _respuesta.ok = true;
                _respuesta.mensaje = "";
                _respuesta.CodFederada = _reg.federada_codigo;
                _respuesta.federada_api = _reg.federada_api;
            }
            catch (Exception ex)
            {
                _respuesta.ok = false;
                _respuesta.mensaje = ex.Message;
            }
            return _respuesta;
        }
        public bool RegistraEn_usuarioClave(Login _Login)
        {
            var _result = false;

            var _rs = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _Login.user.ToUpper());
            var pwd = _encriptador.EncodeMD5(_Login.password);
            var _reg = BD.SSO_usuarioClave.FirstOrDefault(x => x.usuarioClave_usuario_id == _rs.usuario_id && x.usuarioClave_pwd == pwd);
            if (_reg == null)
            {
                var sso_usuarioClave = new SSO_usuarioClave
                {
                    usuarioClave_usuario_id = _rs.usuario_id,
                    usuarioClave_fecha = DateTime.Now,
                    usuarioClave_pwd = pwd
                };

                BD.SSO_usuarioClave.Add(sso_usuarioClave);
                _result = BD.SaveChanges() > 0;

            }
            return _result;
        }

        #region "Metodos privados"
        private bool RegistraEn_SSO_usuario(UsuarioAD _usuarioAD)
        {
            var _result = false;
            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _usuarioAD.usuario_login.ToUpper());
            if (_reg == null)
            {
                var _sso_usuario = new SSO_usuario
                {
                    usuario_login = _usuarioAD.usuario_login,
                    usuario_nombre = _usuarioAD.usuario_nombre,
                    usuario_apPaterno = _usuarioAD.usuario_apPaterno,
                    usuario_apMaterno = _usuarioAD.usuario_apMaterno,
                    usuario_correoUPC = _usuarioAD.usuario_correoUPC,
                    usuario_correoPersonal = string.IsNullOrEmpty(_usuarioAD.Usuario_correoPersonal) ? "" : _usuarioAD.Usuario_correoPersonal,
                    usuario_telefono = string.IsNullOrEmpty(_usuarioAD.usuario_telefono) ? "" : _usuarioAD.usuario_telefono,
                    usuario_estado = true,
                    usuario_flagAsociarRedes = false,
                    usuario_flagRecPas = true,
                    usuario_tipo = _usuarioAD.usuarioTipo
                };

                BD.SSO_usuario.Add(_sso_usuario);
                _result = BD.SaveChanges() > 0;

                //Registra auditoria
                if (_result) _result = RegistraEn_SSO_auditoria(_usuarioAD.usuario_login.ToUpper(), "SSO_USUARIO", "C");

            }
            else
            {
                //Actualiza CorreoPersonal y telefono
                var _entro = false;
                _result = true;

                if (!string.IsNullOrEmpty(_usuarioAD.Usuario_correoPersonal))
                {
                    if (_reg.usuario_correoPersonal != _usuarioAD.Usuario_correoPersonal)
                    {
                        _entro = true;
                    }
                }
                if (!string.IsNullOrEmpty(_usuarioAD.usuario_telefono))
                {
                    if (_reg.usuario_telefono != _usuarioAD.usuario_telefono)
                    {
                        _entro = true;
                    }
                }
                if (_entro)
                {
                    _reg.usuario_telefono = _usuarioAD.usuario_telefono ?? string.Empty;
                    _reg.usuario_correoPersonal = _usuarioAD.Usuario_correoPersonal ?? string.Empty;
                    _result = BD.SaveChanges() > 0;
                    //Registra auditoria
                    if (_result) _result = RegistraEn_SSO_auditoria(_usuarioAD.usuario_login.ToUpper(), "SSO_USUARIO", "U");
                }
            }
            return _result;
        }
        private bool RegistraEn_SSO_usuarioCnx(UsuarioAD _usuarioAD, string codFederada)
        {
            var _result = false;
            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _usuarioAD.usuario_login.ToUpper()); //obtiene el "usuario_id"

            var _obj = new SSO_usuarioCnx
            {
                usuarioCnx_usuario_id = _reg.usuario_id,
                usuarioCnx_fecha = DateTime.Now,
                usuarioCnx_codFederada = codFederada
            };

            BD.SSO_usuarioCnx.Add(_obj);
            _result = BD.SaveChanges() > 0;

            //Registra auditoria
            //if (_result == true) _result = RegistraEn_SSO_auditoria(_usuarioAD.usuario_login.ToUpper(), "SSO_USUARIOCNX", "C");

            return _result;
        }
        private bool RegistraEn_SSO_auditoria(string _usuario, string _tabla, string _operacion)
        {
            var _obj = new SSO_auditoria
            {
                auditoria_tablaNombre = _tabla,
                auditoria_operacion = _operacion,
                auditoria_fechaOperacion = DateTime.Now,
                auditoria_usuario = _usuario
            };

            BD.SSO_auditoria.Add(_obj);
            return BD.SaveChanges() > 0;
        }
        private bool RegistraEn_SSO_code(SSO_code _sso_code, Login _login)
        {
            BD.SSO_code.Add(_sso_code);
            bool _result = BD.SaveChanges() > 0;

            //Registra auditoria
            //if (_result == true) _result = RegistraEn_SSO_auditoria(_login.user.ToUpper(), "SSO_CODE", "C");

            return _result;
        }
        private bool RegistraEn_SSO_usuarioRed(Login _login, Respuesta _claims)
        {
            var _result = false;

            var _reg = BD.SSO_usuario.FirstOrDefault(x => x.usuario_login.ToUpper() == _login.user.ToUpper());

            var _reg_delete = BD.SSO_usuarioRed.FirstOrDefault(x => x.usuarioRed_usuario_id == _reg.usuario_id
                                                                    && x.usuarioRed_redSocial_id == _login.redSocial_id);
            if (!(_reg_delete == null))
            {
                BD.SSO_usuarioRed.Remove(_reg_delete);
                _result = BD.SaveChanges() > 0;
            }
            else
            {
                _result = true;
            }

            if (_result)
            {
                var _reg_insert = new SSO_usuarioRed
                {
                    usuarioRed_usuario_id = _reg.usuario_id,
                    usuarioRed_redSocial_id = _login.redSocial_id,
                    usuarioRed_correo_usuario = _claims.objJwtClaims.email,
                    usuarioRed_nombre_usuario = _claims.objJwtClaims.name,
                    usuarioRed_imagenUrl = _claims.objJwtClaims.picture,
                    usuarioRed_fecha = DateTime.Now
                };

                BD.SSO_usuarioRed.Add(_reg_insert);
                _result = BD.SaveChanges() > 0;
            }

            return _result;
        }
        #endregion

    }
}
