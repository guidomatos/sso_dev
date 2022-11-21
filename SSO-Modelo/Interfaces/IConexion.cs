using System;
using SSO_Modelo.Modelo;
using System.Collections.Generic;
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;

namespace SSO_Modelo.Interfaces
{
    public interface IConexion
    {
        bool RegistraUltActDatos(Login _login);
        bool RegistraUsuarioTipo(Login _login);
        List<SSO_usuario> ListaDeUsuarios();
        bool RegistraFlag(Login _login);
        bool ConsultaFlag(Login _login);
        Respuesta Consulta_SSO_usuarioRed(Login _login);
        Respuesta Consulta_NoMostrar(Login _login);
        Respuesta Consulta_Variables(string nombreVariable);
        Respuesta Consulta_Variables_Id(int variableId);
        Respuesta RegistrarValidaCredenciales(Respuesta _respuesta);
        Respuesta RegisterCodeSend(Login _login);
        Respuesta CodeSendSMS(Login _login);
        Respuesta ProcesaJWT(Login _login);
        Respuesta UpdNoMostrar(Login _login);
        Respuesta EvaluaJWT(Login _login);
        Respuesta CodeEval(Login _login);
        Respuesta ProcesaFace(Login _login);
        Respuesta EvaluaFace(Login _login);
        Respuesta BorraEn_SSO_usuarioRed(Login _login);
        string MensajesAD(int _code);
        Respuesta Consulta_federada(string _codFederada);
        Respuesta Consulta_federada_nom(string _nomFederada);
        bool RegistraEn_usuarioClave(Login _Login);
    }

}