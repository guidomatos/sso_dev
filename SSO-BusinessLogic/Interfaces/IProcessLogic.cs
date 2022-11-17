using SSO_SecurityServerF;
using SSO_SecurityServerF.Clases;

namespace SSO_BusinessLogic.Interfaces
{
    public interface IProcessLogic
    {
        UsuarioAD ConsultaAD(Login _login);
        Respuesta ChangePasswordAD(Login _login);
        Respuesta ValidaCredenciales(Login _login);
        Respuesta ValidaSoloUsuario(Login _login);
        Respuesta CodeSend(Login _login);
        Respuesta CodeSendSMS(Login _login);
        Respuesta EvaluaJWT(Login _login);
        Respuesta EvaluaFace(Login _login);
    }
}