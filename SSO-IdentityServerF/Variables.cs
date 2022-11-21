using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSO_Modelo.Interfaces;

namespace SSO_IdentityServerF
{
    public class Variables
    {
        private static readonly IConexion _conexion;

        public Variables(IConexion conexion)
        {
            _conexion = conexion;
        }

        public static string tokenSecurityKey => _conexion.Consulta_Variables("ssoSecurityKey").mensaje;

        public static string tokenAudience => _conexion.Consulta_Variables("ssoTokenAudience").mensaje;


    }
}
