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

    public partial class SSO_code
    {
        public int code_id { get; set; }
        public int code_usuario_id { get; set; }
        public string code_CodeValue { get; set; }
        public Nullable<System.DateTime> code_fechaRegistro { get; set; }
        public string code_medioEnvio { get; set; }

        public virtual SSO_usuario SSO_usuario { get; set; }
    }
}
