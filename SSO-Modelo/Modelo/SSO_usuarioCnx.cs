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

    public partial class SSO_usuarioCnx
    {
        public int usuarioCnx_id { get; set; }
        public int usuarioCnx_usuario_id { get; set; }
        public DateTime usuarioCnx_fecha { get; set; }
        public string usuarioCnx_codFederada { get; set; }

        public virtual SSO_usuario SSO_usuario { get; set; }
    }
}
