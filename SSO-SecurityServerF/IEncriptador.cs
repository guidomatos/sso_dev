using SSO_SecurityServerF.Clases;

namespace SSO_SecurityServerF
{
    public interface IEncriptador
    {
        string EncodeBase64(string str);

        string DecodeBase64(string str);

        UsuarioAD EncodeClaseBase64(UsuarioAD UsuarioAD);

        UsuarioAD DecodeClaseBase64(UsuarioAD UsuarioAD);

        string Encripta3DES(string stCadena, string stKey);

        string Desencripta3DES(string stCadena, string stKey);

        string EncodeMD5(string stCadena);

    }
}
