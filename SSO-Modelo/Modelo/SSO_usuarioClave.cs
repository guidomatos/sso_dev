//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SSO_Modelo.Modelo
{
    using System;
    using System.Collections.Generic;

    public partial class SSO_usuarioClave
    {
        public int usuarioClave_id { get; set; }
        public int usuarioClave_usuario_id { get; set; }
        public System.DateTime usuarioClave_fecha { get; set; }
        public string usuarioClave_pwd { get; set; }

        public virtual SSO_usuario SSO_usuario { get; set; }
    }
}
