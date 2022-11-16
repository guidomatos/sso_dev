using SSO_SecurityServerF.Clases;
using System;

namespace SSO_SecurityServerF
{
    public class Respuesta
    {
        public bool ok { get; set; }
        public int code { get; set; }  //devuelve codigos de AD del 0 al 7
        public string mensaje { get; set; }
        public UsuarioAD obj { get; set; }
        public JwtClaims objJwtClaims { get; set; }
        public bool noMostrar { get; set; }
        public string federada { get; set; }
        public Boolean flagRecPas { get; set; }
        public string CodFederada { get; set; }
        public string federada_api { get; set; }   // metodo consulta_federada, guarda A:Azure O:On premises 
        public string celular { get; set; }
        public string correo { get; set; }
    }
}
