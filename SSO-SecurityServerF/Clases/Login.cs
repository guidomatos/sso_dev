using System;

namespace SSO_SecurityServerF.Clases
{
    public class Login
    {
        public string user { get; set; }
        public string password { get; set; }
        public string usuario_nombre { get; set; }
        public string Usuario_correoPersonal { get; set; }
        public string usuario_telefono { get; set; }
        public string code { get; set; }
        public string redSocial_token { get; set; } //Google
        public int redSocial_id { get; set; } //Faceboobk y Google
        public string redSocial_name { get; set; } //Faceboobk
        public string redSocial_email { get; set; } //Faceboobk
        public string redSocial_picture { get; set; }  //Faceboobk
        public Boolean noMostrar { get; set; }
        // al momento del login se recibe este dato para grbarlo en bd.conexion
        public string CodFederada { get; set; }
        public string tipoMensaje { get; set; }
        public string usuarioTipo { get; set; }
    }
}
