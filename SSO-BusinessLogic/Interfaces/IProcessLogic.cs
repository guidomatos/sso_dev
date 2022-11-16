using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;

namespace SSO_BusinessLogic.Interfaces
{
    public interface IProcessLogic
    {
        UsuarioAD consultaAD(Login _login);
        Respuesta CodeSend(Login _login);
        Respuesta ValidaCredenciales(Login _login);
        Respuesta ValidaSoloUsuario(Login _login);
        Respuesta CodeSendSMS(Login _login);
        Respuesta ChangePasswordAD(Login _login);
        Respuesta evaluaJWT(Login _login);
        Respuesta evaluaFace(Login _login);
        string Api_Usuarios_Tipo(string _user);
    }
}