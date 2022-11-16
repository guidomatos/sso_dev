using System;
using SSO_Modelo.Clases;
using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;

namespace SSO_Modelo.Interfaces
{
    public interface IConexion
    {
        Boolean registraUltActDatos(Login _login);
        Boolean registraFlag(Login _login);
        Boolean consultaFlag(Login _login);
        Boolean registraEn_SSO_usuario(UsuarioAD _usuarioAD);
        Boolean registraEn_SSO_usuarioCnx(UsuarioAD _usuarioAD, string codFederada);
        Boolean registraEn_SSO_auditoria(string _usuario, string _tabla, string _operacion);
        Respuesta RegistrarValidaCredenciales(Respuesta _respuesta);
        Respuesta procesaJWT(Login _login);
        Respuesta procesaFace(Login _login);
        Respuesta UpdNoMostrar(Login _login);
        Respuesta evaluaJWT(Login _login);
        Respuesta evaluaFace(Login _login);
        Respuesta consulta_SSO_usuarioRed(Login _login);
        Respuesta consulta_NoMostrar(Login _login);
        Respuesta consulta_Variables(string nombreVariable);
        Respuesta consulta_Variables_Id(int variableId);
        Respuesta RegisterCodeSend(Login _login);
        Respuesta CodeEval(Login _login);
        Respuesta BorraEn_SSO_usuarioRed(Login _login);
        Respuesta CodeSendSMS(Login _login);
        Respuesta consulta_federada(string _codFederada);
        Respuesta consulta_federada_nom(string _nomFederada);
        Boolean registraEn_usuarioClave(Login _Login);
    }

}