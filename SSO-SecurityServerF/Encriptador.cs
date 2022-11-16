using System;
using SSO_SecurityServerF.Clases;

using System.Security.Cryptography;
using System.Text;

namespace SSO_SecurityServerF
{
    public class Encriptador:IEncriptador
    {

        public UsuarioAD EncodeClaseBase64(UsuarioAD UsuarioAD)
        {
            UsuarioAD oClase = new UsuarioAD();

            oClase.usuario_login = string.IsNullOrEmpty(UsuarioAD.usuario_login) ? "" : EncodeBase64(UsuarioAD.usuario_login);
            oClase.usuario_clave = string.IsNullOrEmpty(UsuarioAD.usuario_clave) ? "" : EncodeBase64(UsuarioAD.usuario_clave);
            oClase.usuario_nombre = string.IsNullOrEmpty(UsuarioAD.usuario_nombre) ? "" : EncodeBase64(UsuarioAD.usuario_nombre);
            oClase.usuario_apPaterno = string.IsNullOrEmpty(UsuarioAD.usuario_apPaterno) ? "" : EncodeBase64(UsuarioAD.usuario_apPaterno);
            oClase.usuario_apMaterno = string.IsNullOrEmpty(UsuarioAD.usuario_apMaterno) ? "" : EncodeBase64(UsuarioAD.usuario_apMaterno);
            oClase.usuario_correoUPC = string.IsNullOrEmpty(UsuarioAD.usuario_correoUPC) ? "" : EncodeBase64(UsuarioAD.usuario_correoUPC);
            oClase.Usuario_correoPersonal = string.IsNullOrEmpty(UsuarioAD.Usuario_correoPersonal) ? "" : EncodeBase64(UsuarioAD.Usuario_correoPersonal);
            oClase.usuario_telefono = string.IsNullOrEmpty(UsuarioAD.usuario_telefono) ? "" : EncodeBase64(UsuarioAD.usuario_telefono);
            oClase.usuario_code = UsuarioAD.usuario_code;
            oClase.usuario_msg = UsuarioAD.usuario_msg;
            oClase.CodPersona = string.IsNullOrEmpty(UsuarioAD.CodPersona) ? "" : EncodeBase64(UsuarioAD.CodPersona);
            oClase.usuarioTipo = string.IsNullOrEmpty(UsuarioAD.usuarioTipo) ? "" : EncodeBase64(UsuarioAD.usuarioTipo);

            return oClase;
        }

        public UsuarioAD DecodeClaseBase64(UsuarioAD UsuarioAD)
        {
            UsuarioAD oClase = new UsuarioAD();

            oClase.usuario_login          = string.IsNullOrEmpty(UsuarioAD.usuario_login) ? "" : DecodeBase64(UsuarioAD.usuario_login);
            oClase.usuario_clave          = string.IsNullOrEmpty(UsuarioAD.usuario_clave) ? "" : DecodeBase64(UsuarioAD.usuario_clave);
            oClase.usuario_nombre         = string.IsNullOrEmpty(UsuarioAD.usuario_nombre) ? "" : DecodeBase64(UsuarioAD.usuario_nombre);
            oClase.usuario_apPaterno      = string.IsNullOrEmpty(UsuarioAD.usuario_apPaterno) ? "" : DecodeBase64(UsuarioAD.usuario_apPaterno);
            oClase.usuario_apMaterno      = string.IsNullOrEmpty(UsuarioAD.usuario_apMaterno) ? "" : DecodeBase64(UsuarioAD.usuario_apMaterno);
            oClase.usuario_correoUPC      = string.IsNullOrEmpty(UsuarioAD.usuario_correoUPC) ? "" : DecodeBase64(UsuarioAD.usuario_correoUPC);
            oClase.Usuario_correoPersonal = string.IsNullOrEmpty(UsuarioAD.Usuario_correoPersonal) ? "" : DecodeBase64(UsuarioAD.Usuario_correoPersonal);
            oClase.usuario_telefono       = string.IsNullOrEmpty(UsuarioAD.usuario_telefono) ? "" : DecodeBase64(UsuarioAD.usuario_telefono);
            oClase.usuario_code           = UsuarioAD.usuario_code;
            oClase.usuario_msg            = UsuarioAD.usuario_msg;
            oClase.CodPersona             = string.IsNullOrEmpty(UsuarioAD.CodPersona) ? "" : DecodeBase64(UsuarioAD.CodPersona);
            oClase.usuarioTipo            = string.IsNullOrEmpty(UsuarioAD.usuarioTipo) ? "" : DecodeBase64(UsuarioAD.usuarioTipo);

            return oClase;
        }

        public string EncodeBase64(string str)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }

        public string DecodeBase64(string str)
        {
            try
            {
                byte[] decbuff = Convert.FromBase64String(str);
                return System.Text.Encoding.UTF8.GetString(decbuff);
            }
            catch
            {
                //si se envia una cadena si codificación base64, mandamos vacio
                return "";
            }
        }

        public string Encripta3DES(string stCadena, string stKey)
        {
            
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(stKey));
            hashmd5 = null;

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = keyhash;
            des.Mode = CipherMode.ECB;

            byte[] buff = ASCIIEncoding.ASCII.GetBytes(stCadena);
            string stEncrypted = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));

            return stEncrypted;
        }

        public string Desencripta3DES(string stCadena, string stKey)
        {

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(stKey));
            hashmd5 = null;

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = keyhash;
            des.Mode = CipherMode.ECB;

            byte[] buff = Convert.FromBase64String(stCadena);
            string stDecrypted = ASCIIEncoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));

            return stDecrypted;
        }

        public string EncodeMD5(string stCadena)
        {
            byte[] BtclearBytes;
            BtclearBytes = new UnicodeEncoding().GetBytes(stCadena);
            byte[] BthashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(BtclearBytes);
            string sthashedText = BitConverter.ToString(BthashedBytes);
            return sthashedText;
        }

    }
}
